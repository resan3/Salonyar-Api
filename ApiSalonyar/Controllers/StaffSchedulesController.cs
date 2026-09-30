using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
namespace ApiSalonyar.Controllers
{
  
   

    [ApiController]
    [Route("api/[controller]")]
    public class StaffSchedulesController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public StaffSchedulesController(ClinicDbContext context) => _context = context;

        // GET api/staffschedules?staffId=1&year=1403&month=7
        [HttpGet]
        public async Task<IActionResult> GetByStaffAndMonth(int staffId, int year, int month)
        {
            // تبدیل سال/ماه شمسی به میلادی برای فیلتر
            // فعلاً با میلادی کار می‌کنیم
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1).AddDays(-1);

            var list = await _context.StaffSchedules
                .Where(x => x.StaffId == staffId
                         && !x.IsDeleted
                         && x.ScheduleDate >= from
                         && x.ScheduleDate <= to)
                .OrderBy(x => x.ScheduleDate)
                .ToListAsync();

            return Ok(list);
        }

        // POST api/staffschedules
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] StaffSchedule item)
        {
            var existing = await _context.StaffSchedules
                .FirstOrDefaultAsync(x => x.StaffId == item.StaffId
                                       && x.ScheduleDate == item.ScheduleDate
                                       && !x.IsDeleted);
            if (existing != null)
            {
                existing.IsWorkDay = item.IsWorkDay;
                existing.StartTime = item.StartTime;
                existing.EndTime = item.EndTime;
                existing.Note = item.Note;
            }
            else
            {
                item.CreatedAt = DateTime.Now;
                _context.StaffSchedules.Add(item);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/staffschedules/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.StaffSchedules.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
