using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiseasesController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public DiseasesController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/Diseases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Disease>>> GetDiseases()
        {
            return await _context.Diseases
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        // GET: api/Diseases/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Disease>> GetDisease(int id)
        {
            var disease = await _context.Diseases.FindAsync(id);

            if (disease == null)
                return NotFound();

            return disease;
        }

        // POST: api/Diseases
        [HttpPost]
        public async Task<ActionResult<Disease>> PostDisease(Disease disease)
        {
            _context.Diseases.Add(disease);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDisease),
                new { id = disease.DiseaseId },
                disease);
        }

        // PUT: api/Diseases/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDisease(int id, Disease disease)
        {
            if (id != disease.DiseaseId)
                return BadRequest();

            _context.Entry(disease).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Diseases.AnyAsync(x => x.DiseaseId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Diseases/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDisease(int id)
        {
            var disease = await _context.Diseases.FindAsync(id);

            if (disease == null)
                return NotFound();

            _context.Diseases.Remove(disease);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}