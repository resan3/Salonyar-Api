
using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferralSourcesController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public ReferralSourcesController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/ReferralSources
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReferralSource>>> GetReferralSources()
        {
            return await _context.ReferralSources
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        // GET: api/ReferralSources/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReferralSource>> GetReferralSource(int id)
        {
            var referralSource = await _context.ReferralSources.FindAsync(id);

            if (referralSource == null)
                return NotFound();

            return referralSource;
        }

        // POST: api/ReferralSources
        [HttpPost]
        public async Task<ActionResult<ReferralSource>> PostReferralSource(ReferralSource referralSource)
        {
            _context.ReferralSources.Add(referralSource);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetReferralSource),
                new { id = referralSource.ReferralSourceId },
                referralSource);
        }

        // PUT: api/ReferralSources/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReferralSource(int id, ReferralSource referralSource)
        {
            if (id != referralSource.ReferralSourceId)
                return BadRequest();

            _context.Entry(referralSource).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.ReferralSources.AnyAsync(x => x.ReferralSourceId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/ReferralSources/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReferralSource(int id)
        {
            var referralSource = await _context.ReferralSources.FindAsync(id);

            if (referralSource == null)
                return NotFound();

            _context.ReferralSources.Remove(referralSource);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
