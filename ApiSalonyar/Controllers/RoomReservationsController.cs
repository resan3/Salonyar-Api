using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomReservationsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public RoomReservationsController(ClinicDbContext context) => _context = context;

        // یه متد کمکی برای چک تداخل - بالای کنترلر اضافه کن
        private async Task<bool> HasConflict(int roomId, int staffId, DateTime date, TimeSpan start, TimeSpan end, int? excludeId = null)
        {
            var query = _context.RoomReservations
                .Where(x => !x.IsDeleted &&
                            x.ReservationDate == date &&
                            x.ReservationStatusId != 4 && // لغو شده رو نادیده بگیر
                            (
                                // تداخل اتاق
                                (x.RoomId == roomId && x.StartTime < end && x.EndTime > start)
                                ||
                                // تداخل همکار
                                (x.StaffId == staffId && x.StartTime < end && x.EndTime > start)
                            ));

            if (excludeId.HasValue)
                query = query.Where(x => x.ReservationId != excludeId.Value);

            return await query.AnyAsync();
        }

        // GET: api/RoomReservations?date=2025-04-01
        // این Endpoint برای نمای شماتیک استفاده میشه
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomReservation>>> GetReservations(
            [FromQuery] DateTime? date,
            [FromQuery] int? roomId)
        {
            var query = _context.RoomReservations
                .Include(x => x.Patient)
                .Include(x => x.Staff)
                .Include(x => x.Room)
                .Include(x => x.Treatment)
                .Include(x => x.ReservationStatus)
                .Where(x => !x.IsDeleted);

            if (date.HasValue)
                query = query.Where(x => x.ReservationDate == date.Value); // ✅ مستقیم مقایسه

            if (roomId.HasValue)
                query = query.Where(x => x.RoomId == roomId.Value);

            return await query
                .OrderBy(x => x.RoomId)
                .ThenBy(x => x.StartTime)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomReservation>> GetReservation(int id)
        {
            var item = await _context.RoomReservations
                .Include(x => x.Patient)
                .Include(x => x.Staff)
                .Include(x => x.Room)
                .Include(x => x.Treatment)
                .Include(x => x.ReservationStatus)
                .FirstOrDefaultAsync(x => x.ReservationId == id && !x.IsDeleted);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<RoomReservation>> PostReservation(RoomReservation item)
        {
            ModelState.Clear();
            item.IsDeleted = false;
            item.CreatedAt = DateTime.Now;
            item.BranchId = item.BranchId == 0 ? 1 : item.BranchId;
            // ✅ چک تداخل
            if (await HasConflict(item.RoomId, item.StaffId, item.ReservationDate, item.StartTime, item.EndTime))
                return BadRequest("تداخل زمانی وجود دارد. این اتاق یا همکار در این بازه زمانی رزرو دارد.");

            // وضعیت پیش‌فرض: تایید شده (id=2 بر اساس داده‌ای که قبلاً Insert کردیم)
            if (item.ReservationStatusId == 0)
                item.ReservationStatusId = 2;

            _context.RoomReservations.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReservation), new { id = item.ReservationId }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, [FromBody] RoomReservation item)
        {
            ModelState.Clear();
            var existing = await _context.RoomReservations.FindAsync(id);
            if (existing == null) return NotFound();

            // ✅ چک تداخل - رزرو فعلی رو از چک خارج کن
            if (await HasConflict(item.RoomId, item.StaffId, item.ReservationDate, item.StartTime, item.EndTime, id))
                return BadRequest("تداخل زمانی وجود دارد. این اتاق یا همکار در این بازه زمانی رزرو دارد.");

            existing.PatientId = item.PatientId;
            existing.StaffId = item.StaffId;
            existing.RoomId = item.RoomId;
            existing.TreatmentId = item.TreatmentId;
            existing.ReservationStatusId = item.ReservationStatusId;
            existing.ReservationDate = item.ReservationDate;
            existing.Notes = item.Notes;

            // TimeSpan رو جدا parse کن
            if (TimeSpan.TryParse(item.StartTime.ToString(), out var start))
                existing.StartTime = start;
            if (TimeSpan.TryParse(item.EndTime.ToString(), out var end))
                existing.EndTime = end;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // تغییر فقط وضعیت رزرو (مثلاً از تایید شده به انجام شده)
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> PatchStatus(int id, [FromBody] int statusId)
        {
            var item = await _context.RoomReservations.FindAsync(id);
            if (item == null) return NotFound();
            item.ReservationStatusId = statusId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var item = await _context.RoomReservations.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
