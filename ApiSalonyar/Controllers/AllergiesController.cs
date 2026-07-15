using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllergiesController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public AllergiesController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/Allergies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Allergy>>> GetAllergies()
        {
            return await _context.Allergies
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        // GET: api/Allergies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Allergy>> GetAllergy(int id)
        {
            var allergy = await _context.Allergies.FindAsync(id);

            if (allergy == null)
                return NotFound();

            return allergy;
        }

        // POST: api/Allergies
        [HttpPost]
        public async Task<ActionResult<Allergy>> PostAllergy(Allergy allergy)
        {
            _context.Allergies.Add(allergy);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAllergy),
                new { id = allergy.AllergyId },
                allergy);
        }

        // PUT: api/Allergies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAllergy(int id, Allergy allergy)
        {
            if (id != allergy.AllergyId)
                return BadRequest();

            _context.Entry(allergy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Allergies.AnyAsync(x => x.AllergyId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Allergies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAllergy(int id)
        {
            var allergy = await _context.Allergies.FindAsync(id);

            if (allergy == null)
                return NotFound();

            _context.Allergies.Remove(allergy);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}