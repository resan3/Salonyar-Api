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



        // گزارش مراجعات بیماران - فیلتر بر اساس تاریخ
        [HttpGet("patient-visits-report")]
        public async Task<ActionResult<object>> PatientVisitsReport(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int? patientId)
        {
            var query = _context.PatientVisits
                .Include(x => x.Patient)
                .Include(x => x.Staff)
                .Include(x => x.Treatment)
                .Where(x => !x.IsDeleted);

            if (from.HasValue) query = query.Where(x => x.VisitDate >= from.Value);
            if (to.HasValue) query = query.Where(x => x.VisitDate <= to.Value);
            if (patientId.HasValue) query = query.Where(x => x.PatientId == patientId.Value);

            var visits = await query.OrderByDescending(x => x.VisitDate).ToListAsync();

            return Ok(new
            {
                totalVisits = visits.Count,
                visits = visits,
                byTreatment = visits.GroupBy(x => x.Treatment?.Title ?? "نامشخص")
                                      .Select(g => new { name = g.Key, count = g.Count() })
                                      .OrderByDescending(x => x.count).ToList(),
            });
        }

        // گزارش اشغال اتاق‌ها
        [HttpGet("room-occupancy-report")]
        public async Task<ActionResult<object>> RoomOccupancyReport(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var fromDate = from ?? DateTime.Today.AddDays(-30);
            var toDate = to ?? DateTime.Today;

            var reservations = await _context.RoomReservations
                .Include(x => x.Room)
                .Where(x => !x.IsDeleted &&
                    x.ReservationDate >= fromDate &&
                    x.ReservationDate <= toDate)
                .ToListAsync();

            var rooms = await _context.Rooms
                .Where(x => !x.IsDeleted && x.IsActive == true)
                .ToListAsync();

            var totalDays = (toDate - fromDate).Days + 1;

            var byRoom = rooms.Select(r => {
                var roomRes = reservations.Where(x => x.RoomId == r.RoomId).ToList();
                return new
                {
                    roomName = r.Title,
                    totalReservations = roomRes.Count,
                    completedCount = roomRes.Count(x => x.ReservationStatusId == 3),
                    cancelledCount = roomRes.Count(x => x.ReservationStatusId == 4),
                    occupancyDays = roomRes.Select(x => x.ReservationDate).Distinct().Count(),
                    occupancyPercent = totalDays > 0
                        ? (int)Math.Round((double)roomRes.Select(x => x.ReservationDate).Distinct().Count() / totalDays * 100)
                        : 0,
                };
            }).ToList();

            return Ok(new
            {
                fromDate = fromDate,
                toDate = toDate,
                totalReservations = reservations.Count,
                byRoom = byRoom,
                byStatus = reservations
                    .GroupBy(x => x.ReservationStatusId)
                    .Select(g => new { statusId = g.Key, count = g.Count() })
                    .ToList(),
            });
        }

        // گزارش عملکرد کارکنان
        [HttpGet("staff-performance-report")]
        public async Task<ActionResult<object>> StaffPerformanceReport(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var fromDate = from ?? DateTime.Today.AddDays(-30);
            var toDate = to ?? DateTime.Today;

            var visits = await _context.PatientVisits
                .Include(x => x.Staff)
                .Include(x => x.Treatment)
                .Where(x => !x.IsDeleted &&
                    x.VisitDate >= fromDate &&
                    x.VisitDate <= toDate)
                .ToListAsync();

            var reservations = await _context.RoomReservations
                .Include(x => x.Staff)
                .Where(x => !x.IsDeleted &&
                    x.ReservationDate >= fromDate &&
                    x.ReservationDate <= toDate)
                .ToListAsync();

            var staffList = await _context.staff
                .Where(x => !x.IsDeleted && x.IsActive == true)
                .ToListAsync();

            var byStaff = staffList.Select(s => {
                var staffVisits = visits.Where(x => x.StaffId == s.StaffId).ToList();
                var staffRes = reservations.Where(x => x.StaffId == s.StaffId).ToList();
                return new
                {
                    staffName = s.FullName,
                    totalVisits = staffVisits.Count,
                    totalReservations = staffRes.Count,
                    completedRes = staffRes.Count(x => x.ReservationStatusId == 3),
                    cancelledRes = staffRes.Count(x => x.ReservationStatusId == 4),
                    topTreatments = staffVisits
                        .GroupBy(x => x.Treatment?.Title ?? "نامشخص")
                        .Select(g => new { name = g.Key, count = g.Count() })
                        .OrderByDescending(x => x.count)
                        .Take(3)
                        .ToList(),
                };
            })
            .Where(x => x.totalVisits > 0 || x.totalReservations > 0)
            .OrderByDescending(x => x.totalVisits)
            .ToList();

            return Ok(new
            {
                fromDate = fromDate,
                toDate = toDate,
                totalVisits = visits.Count,
                byStaff = byStaff,
            });
        }
    }
}
