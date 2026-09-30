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
        [HttpGet]
        [HttpGet]
        public async Task<ActionResult> GetReservations(
    [FromQuery] DateTime? date,
    [FromQuery] int? roomId)
        {
            var query = _context.RoomReservations
                .Where(x => !x.IsDeleted);

            if (date.HasValue)
                query = query.Where(x =>
                    x.ReservationDate.Year == date.Value.Year &&
                    x.ReservationDate.Month == date.Value.Month &&
                    x.ReservationDate.Day == date.Value.Day);

            if (roomId.HasValue)
                query = query.Where(x => x.RoomId == roomId.Value);

            var result = await query
                .OrderBy(x => x.RoomId)
                .ThenBy(x => x.StartTime)
                .Select(x => new {
                    x.ReservationId,
                    x.PatientId,
                    x.StaffId,
                    x.RoomId,
                    x.TreatmentId,
                    x.ReservationStatusId,
                    x.ReservationDate,
                    x.StartTime,
                    x.EndTime,
                    x.Notes,
                    x.StaffIsFixed,
                    x.StaffFixedNote,
                    x.VisitId,
            // فقط فیلدهای لازم از Navigation Properties
                    Patient = x.Patient == null ? null : new
                    {
                        x.Patient.PatientId,
                        x.Patient.FirstName,
                        x.Patient.LastName,
                        x.Patient.Mobile
                    },
                    Staff = x.Staff == null ? null : new
                    {
                        x.Staff.StaffId,
                        x.Staff.FullName
                    },
                    Room = x.Room == null ? null : new
                    {
                        x.Room.RoomId,
                        x.Room.Title
                    },
                    Treatment = x.Treatment == null ? null : new
                    {
                        x.Treatment.TreatmentId,
                        x.Treatment.Title,
                        x.Treatment.Price
                    },
                    ReservationStatus = x.ReservationStatus == null ? null : new
                    {
                        x.ReservationStatus.ReservationStatusId,
                        x.ReservationStatus.Title,
                        x.ReservationStatus.ColorHex
                    },


                    // ✅ اطلاعات فاکتور برای نمایش وضعیت پرداخت
                    Invoice = _context.Invoices
            .Where(inv => inv.ReservationId == x.ReservationId && !inv.IsDeleted)
            .Select(inv => new {
                inv.InvoiceId,
                inv.FinalAmount,
                inv.PaidAmount,
                inv.RemainingAmount,
                inv.Status
            })
            .FirstOrDefault(),
                })
                .ToListAsync();

            return Ok(result);
        }






























        // GET: api/RoomReservations?date=2025-04-01
        // این Endpoint برای نمای شماتیک استفاده میشه
        /*        [HttpGet]
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
                }*/

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
            // چک شیفت کاری همکار
            if (item.StaffId > 0)
            {
                var schedule = await _context.StaffSchedules
                    .FirstOrDefaultAsync(s => s.StaffId == item.StaffId
                        && s.ScheduleDate.Date == item.ReservationDate.Date
                        && !s.IsDeleted);

                if (schedule != null && !schedule.IsWorkDay)
                    return BadRequest("این همکار در این تاریخ شیفت کاری ندارد.");
            }
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
            if (item == null)
                return BadRequest("BODY IS NULL");
            ModelState.Clear();
            var existing = await _context.RoomReservations.FindAsync(id);
            if (existing == null) return NotFound();

            // ✅ چک تداخل - رزرو فعلی رو از چک خارج کن
            if (await HasConflict(item.RoomId, item.StaffId, item.ReservationDate, item.StartTime, item.EndTime, id))
                return BadRequest("تداخل زمانی وجود دارد. این اتاق یا همکار در این بازه زمانی رزرو دارد.");

            // ✅ اگه قبلاً تثبیت شده، همکار رو نمیشه تغییر داد
            if (existing.StaffIsFixed && existing.StaffId != item.StaffId)
                return BadRequest("همکار این رزرو تثبیت شده و قابل تغییر نیست.");

            existing.PatientId = item.PatientId;
            existing.StaffId = item.StaffId;
            existing.RoomId = item.RoomId;
            existing.TreatmentId = item.TreatmentId;
            existing.ReservationStatusId = item.ReservationStatusId;
            existing.ReservationDate = item.ReservationDate;
            existing.Notes = item.Notes;
            existing.StaffIsFixed = item.StaffIsFixed;
            existing.StaffFixedNote = item.StaffFixedNote;
            // TimeSpan رو جدا parse کن
            if (TimeSpan.TryParse(item.StartTime.ToString(), out var start))
                existing.StartTime = start;
            if (TimeSpan.TryParse(item.EndTime.ToString(), out var end))
                existing.EndTime = end;

            await _context.SaveChangesAsync();



            // ✅ اگه فاکتور برای این رزرو وجود داره، آپدیت کن
            if (item.TreatmentId.HasValue)
            {
                var invoice = await _context.Invoices
                    .Include(x => x.InvoiceItems)
                    .FirstOrDefaultAsync(x => x.ReservationId == id && !x.IsDeleted);

                if (invoice != null)
                {
                    var treatment = await _context.Treatments.FindAsync(item.TreatmentId.Value);
                    if (treatment != null)
                    {
                        // آپدیت آیتم‌های فاکتور
                        _context.InvoiceItems.RemoveRange(invoice.InvoiceItems);
                        _context.InvoiceItems.Add(new InvoiceItem
                        {
                            InvoiceId = invoice.InvoiceId,
                            TreatmentId = treatment.TreatmentId,
                            Description = treatment.Title,
                            Quantity = 1,
                            UnitPrice = treatment.Price,
                            DiscountPercent = 0,
                            DiscountAmount = 0,
                            TotalPrice = treatment.Price,
                        });

                        // آپدیت مبالغ فاکتور
                        invoice.TotalAmount = treatment.Price;
                        invoice.DiscountAmount = 0;
                        invoice.TaxAmount = 0;
                        invoice.FinalAmount = treatment.Price;
                        invoice.RemainingAmount = treatment.Price - invoice.PaidAmount;
                        invoice.UpdatedAt = DateTime.Now;

                        await _context.SaveChangesAsync();
                    }
                }
            }


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
