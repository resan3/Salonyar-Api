using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GendersController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public GendersController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/Genders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gender>>> GetGenders()
        {
            return await _context.Genders
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        // GET: api/Genders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gender>> GetGender(int id)
        {
            var gender = await _context.Genders.FindAsync(id);

            if (gender == null)
                return NotFound();

            return gender;
        }

        // POST: api/Genders
        [HttpPost]
        public async Task<ActionResult<Gender>> PostGender(Gender gender)
        {
            _context.Genders.Add(gender);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGender),
                new { id = gender.GenderId },
                gender);
        }

        // PUT: api/Genders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGender(int id, Gender gender)
        {
            if (id != gender.GenderId)
                return BadRequest();

            _context.Entry(gender).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Genders.AnyAsync(x => x.GenderId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Genders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGender(int id)
        {
            var gender = await _context.Genders.FindAsync(id);

            if (gender == null)
                return NotFound();

            _context.Genders.Remove(gender);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}