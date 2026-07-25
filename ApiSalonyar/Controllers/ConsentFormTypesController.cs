using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsentFormTypesController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public ConsentFormTypesController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsentFormType>>> GetTypes()
            => await _context.ConsentFormTypes.OrderBy(x => x.Title).ToListAsync();

        [HttpPost]
        public async Task<ActionResult<ConsentFormType>> PostType(ConsentFormType item)
        {
            ModelState.Clear();
            _context.ConsentFormTypes.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutType(int id, ConsentFormType item)
        {
            ModelState.Clear();
            var existing = await _context.ConsentFormTypes.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Title = item.Title;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteType(int id)
        {
            var item = await _context.ConsentFormTypes.FindAsync(id);
            if (item == null) return NotFound();
            _context.ConsentFormTypes.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
