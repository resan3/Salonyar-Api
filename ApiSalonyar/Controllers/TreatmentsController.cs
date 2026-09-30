using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public TreatmentsController(ClinicDbContext context) => _context = context;

        // همه خدمات (اختیاری: فیلتر بر اساس گروه)
        [HttpGet]
        public async Task<ActionResult> GetTreatments([FromQuery] int? groupId)
        {
            var query = _context.Treatments.Where(x => !x.IsDeleted && x.IsActive == true);
            if (groupId.HasValue)
                query = query.Where(x => x.TreatmentGroupId == groupId);

            var result = await query
                .OrderBy(x => x.TreatmentGroupId)
                .ThenBy(x => x.Title)
                .Select(x => new {
                    x.TreatmentId,
                    x.Title,
                    x.Price,
                    x.TreatmentGroupId,
                    x.IsActive,
                    GroupTitle = x.TreatmentGroup == null ? null : x.TreatmentGroup.Title
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Treatment>> GetTreatment(int id)
        {
            var item = await _context.Treatments
                .Include(x => x.TreatmentGroup)
                .FirstOrDefaultAsync(x => x.TreatmentId == id && !x.IsDeleted);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Treatment>> PostTreatment(Treatment item)
        {
            ModelState.Clear();
            item.IsDeleted = false;
            item.IsActive = true;
            _context.Treatments.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTreatment), new { id = item.TreatmentId }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTreatment(int id, Treatment item)
        {
            ModelState.Clear();
            if (id != item.TreatmentId) return BadRequest();
            var existing = await _context.Treatments.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = item.Title;
            existing.Price = item.Price;
            existing.TreatmentGroupId = item.TreatmentGroupId;
            existing.IsActive = item.IsActive;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTreatment(int id)
        {
            var item = await _context.Treatments.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
