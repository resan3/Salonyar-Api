using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public DashboardController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<object>> GetStats()
        {
            var today = DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            // تعداد کل بیماران
            var totalPatients = await _context.Patients
                .CountAsync(x => !x.IsDeleted);

            // بیماران جدید این هفته
            var newPatientsThisWeek = await _context.Patients
                .CountAsync(x => !x.IsDeleted && x.CreatedAt >= weekStart);

            // رزروهای امروز
            var todayReservations = await _context.RoomReservations
                .CountAsync(x => !x.IsDeleted &&
                    x.ReservationDate.Year == today.Year &&
                    x.ReservationDate.Month == today.Month &&
                    x.ReservationDate.Day == today.Day);

            // رزروهای امروز بر اساس وضعیت
            var todayByStatus = await _context.RoomReservations
                .Where(x => !x.IsDeleted &&
                    x.ReservationDate.Year == today.Year &&
                    x.ReservationDate.Month == today.Month &&
                    x.ReservationDate.Day == today.Day)
                .GroupBy(x => x.ReservationStatusId)
                .Select(g => new { statusId = g.Key, count = g.Count() })
                .ToListAsync();

            // مراجعات این ماه
            var visitsThisMonth = await _context.PatientVisits
                .CountAsync(x => !x.IsDeleted && x.VisitDate >= monthStart);

            // پرکارترین خدمات این ماه
            var topTreatments = await _context.PatientVisits
                .Where(x => !x.IsDeleted && x.VisitDate >= monthStart)
                .GroupBy(x => x.TreatmentId)
                .Select(g => new {
                    treatmentId = g.Key,
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .Take(5)
                .Join(_context.Treatments,
                    v => v.treatmentId,
                    t => t.TreatmentId,
                    (v, t) => new { t.Title, v.count })
                .ToListAsync();

            // رزروهای ۷ روز آینده
            var upcomingReservations = await _context.RoomReservations
                .Include(x => x.Patient)
                .Include(x => x.Treatment)
                .Include(x => x.Room)
                .Where(x => !x.IsDeleted &&
                    x.ReservationDate >= today &&
                    x.ReservationDate <= today.AddDays(7) &&
                    x.ReservationStatusId != 4) // لغو شده نباشه
                .OrderBy(x => x.ReservationDate)
                .ThenBy(x => x.StartTime)
                .Take(8)
                .ToListAsync();

            // اشغال اتاق‌ها امروز
            var totalRooms = await _context.Rooms.CountAsync(x => !x.IsDeleted && x.IsActive == true);
            var occupiedRooms = await _context.RoomReservations
                .Where(x => !x.IsDeleted &&
                    x.ReservationDate.Year == today.Year &&
                    x.ReservationDate.Month == today.Month &&
                    x.ReservationDate.Day == today.Day &&
                    x.ReservationStatusId != 4)
                .Select(x => x.RoomId)
                .Distinct()
                .CountAsync();

            return Ok(new
            {
                totalPatients,
                newPatientsThisWeek,
                todayReservations,
                todayByStatus,
                visitsThisMonth,
                topTreatments,
                upcomingReservations,
                totalRooms,
                occupiedRooms,
                occupancyPercent = totalRooms > 0
                    ? (int)Math.Round((double)occupiedRooms / totalRooms * 100)
                    : 0,
            });
        }
    }
}
