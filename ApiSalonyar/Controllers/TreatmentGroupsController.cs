using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentGroupsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public TreatmentGroupsController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TreatmentGroup>>> GetTreatmentGroups()
            => await _context.TreatmentGroups
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.TreatmentGroupId)
                .ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<TreatmentGroup>> GetTreatmentGroup(int id)
        {
            var item = await _context.TreatmentGroups
                .FirstOrDefaultAsync(x => x.TreatmentGroupId == id && !x.IsDeleted);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<TreatmentGroup>> PostTreatmentGroup(TreatmentGroup item)
        {
            ModelState.Clear();
            item.IsDeleted = false;
            item.IsActive = true;
            _context.TreatmentGroups.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTreatmentGroup), new { id = item.TreatmentGroupId }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTreatmentGroup(int id, TreatmentGroup item)
        {
            ModelState.Clear();
            if (id != item.TreatmentGroupId) return BadRequest();
            var existing = await _context.TreatmentGroups.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = item.Title;
            existing.IsActive = item.IsActive;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTreatmentGroup(int id)
        {
            var item = await _context.TreatmentGroups.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
