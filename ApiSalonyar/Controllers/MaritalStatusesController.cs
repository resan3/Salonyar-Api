using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaritalStatusesController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public MaritalStatusesController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/MaritalStatuses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaritalStatus>>> GetMaritalStatuses()
        {
            return await _context.MaritalStatuses
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        // GET: api/MaritalStatuses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaritalStatus>> GetMaritalStatus(int id)
        {
            var maritalStatus = await _context.MaritalStatuses.FindAsync(id);

            if (maritalStatus == null)
                return NotFound();

            return maritalStatus;
        }

        // POST: api/MaritalStatuses
        [HttpPost]
        public async Task<ActionResult<MaritalStatus>> PostMaritalStatus(MaritalStatus maritalStatus)
        {
            _context.MaritalStatuses.Add(maritalStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMaritalStatus),
                new { id = maritalStatus.MaritalStatusId },
                maritalStatus);
        }

        // PUT: api/MaritalStatuses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaritalStatus(int id, MaritalStatus maritalStatus)
        {
            if (id != maritalStatus.MaritalStatusId)
                return BadRequest();

            _context.Entry(maritalStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.MaritalStatuses.AnyAsync(x => x.MaritalStatusId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/MaritalStatuses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaritalStatus(int id)
        {
            var maritalStatus = await _context.MaritalStatuses.FindAsync(id);

            if (maritalStatus == null)
                return NotFound();

            _context.MaritalStatuses.Remove(maritalStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}