using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsentFormTypesController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public ConsentFormTypesController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/ConsentFormTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsentFormType>>> GetConsentFormTypes()
        {
            return await _context.ConsentFormTypes
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        // GET: api/ConsentFormTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ConsentFormType>> GetConsentFormType(int id)
        {
            var consentFormType = await _context.ConsentFormTypes.FindAsync(id);

            if (consentFormType == null)
                return NotFound();

            return consentFormType;
        }

        // POST: api/ConsentFormTypes
        [HttpPost]
        public async Task<ActionResult<ConsentFormType>> PostConsentFormType(ConsentFormType consentFormType)
        {
            _context.ConsentFormTypes.Add(consentFormType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetConsentFormType),
                new { id = consentFormType.ConsentFormTypeId },
                consentFormType);
        }

        // PUT: api/ConsentFormTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConsentFormType(int id, ConsentFormType consentFormType)
        {
            if (id != consentFormType.ConsentFormTypeId)
                return BadRequest();

            _context.Entry(consentFormType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.ConsentFormTypes.AnyAsync(x => x.ConsentFormTypeId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/ConsentFormTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsentFormType(int id)
        {
            var consentFormType = await _context.ConsentFormTypes.FindAsync(id);

            if (consentFormType == null)
                return NotFound();

            _context.ConsentFormTypes.Remove(consentFormType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}