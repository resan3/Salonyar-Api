using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageTemplatesController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public PackageTemplatesController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PackageTemplate>>> GetTemplates()
            => await _context.PackageTemplates
                .Include(x => x.Treatment)
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Title)
                .ToListAsync();

        [HttpPost]
        public async Task<ActionResult<PackageTemplate>> PostTemplate(PackageTemplate item)
        {
            ModelState.Clear();
            item.IsDeleted = false;
            item.IsActive = true;
            item.CreatedAt = DateTime.Now;
            item.BranchId = item.BranchId == 0 ? 1 : item.BranchId;
            _context.PackageTemplates.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTemplate(int id, PackageTemplate item)
        {
            ModelState.Clear();
            var existing = await _context.PackageTemplates.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Title = item.Title;
            existing.PackageType = item.PackageType;
            existing.TreatmentId = item.TreatmentId;
            existing.SessionCount = item.SessionCount;
            existing.CreditAmount = item.CreditAmount;
            existing.Price = item.Price;
            existing.ValidityDays = item.ValidityDays;
            existing.Description = item.Description;
            existing.IsActive = item.IsActive;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var item = await _context.PackageTemplates.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
