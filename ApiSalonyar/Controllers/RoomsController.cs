using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public RoomsController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
            => await _context.Rooms
                .Where(x => !x.IsDeleted && (x.IsActive == true))
                .OrderBy(x => x.RoomId)
                .ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var item = await _context.Rooms.FirstOrDefaultAsync(x => x.RoomId == id && !x.IsDeleted);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom(Room item)
        {
            ModelState.Clear();
            item.IsDeleted = false;
            item.IsActive = true;
            item.CreatedAt = DateTime.Now;
            item.BranchId = item.BranchId == 0 ? 1 : item.BranchId;
            _context.Rooms.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRoom), new { id = item.RoomId }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, Room item)
        {
            ModelState.Clear();
            if (id != item.RoomId) return BadRequest();
            var existing = await _context.Rooms.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Title = item.Title;
            existing.Area = item.Area;
            existing.IsActive = item.IsActive;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var item = await _context.Rooms.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
