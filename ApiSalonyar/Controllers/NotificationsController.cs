using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public NotificationsController(ClinicDbContext context) => _context = context;

        // GET: api/Notifications?staffId=2
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications(
            [FromQuery] int? staffId,
            [FromQuery] bool? unreadOnly)
        {
            var query = _context.Notifications
                .Include(x => x.Staff)
                .Include(x => x.Reservation)
                    .ThenInclude(x => x.Patient)
                .Include(x => x.Reservation)
                    .ThenInclude(x => x.Treatment)
                .AsQueryable();

            if (staffId.HasValue)
                query = query.Where(x => x.StaffId == staffId.Value);

            if (unreadOnly == true)
                query = query.Where(x => !x.IsRead);

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Take(50)
                .ToListAsync();
        }

        // POST: ساخت اعلان جدید
        [HttpPost]
        public async Task<ActionResult<Notification>> PostNotification(Notification item)
        {
            ModelState.Clear();
            item.CreatedAt = DateTime.Now;
            item.IsRead = false;
            _context.Notifications.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        // PATCH: خوندن اعلان
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var item = await _context.Notifications.FindAsync(id);
            if (item == null) return NotFound();
            item.IsRead = true;
            item.ReadAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: انجام اکشن (تایید حضور یا لغو)
        [HttpPatch("{id}/action")]
        public async Task<IActionResult> DoAction(int id, [FromBody] ActionDto dto)
        {
            ModelState.Clear();
            var notif = await _context.Notifications
                .Include(x => x.Reservation)
                .FirstOrDefaultAsync(x => x.NotificationId == id);
            if (notif == null) return NotFound();

            notif.IsRead = true;
            notif.ReadAt = DateTime.Now;
            notif.ActionDoneAt = DateTime.Now;

            // آپدیت وضعیت رزرو
            if (notif.ReservationId.HasValue)
            {
                var reservation = await _context.RoomReservations.FindAsync(notif.ReservationId.Value);
                if (reservation != null)
                {
                    reservation.ReservationStatusId = dto.NewStatusId;
                    // ذخیره اینکه چه کسی این تغییر رو داده
                    if (dto.ChangedByUserId.HasValue)
                        reservation.CreatedByUserId = dto.ChangedByUserId;
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: تعداد اعلان‌های خوانده نشده
        [HttpGet("unread-count")]
        public async Task<ActionResult<object>> GetUnreadCount([FromQuery] int staffId)
        {
            var count = await _context.Notifications
                .CountAsync(x => x.StaffId == staffId && !x.IsRead);
            return Ok(new { count });
        }
    }

    public class ActionDto
    {
        public int NewStatusId { get; set; }
        public int? ChangedByUserId { get; set; }
        public string? Notes { get; set; }
    }

}
