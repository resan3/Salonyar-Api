
using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodTypesController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public BloodTypesController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/BloodTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BloodType>>> GetBloodTypes()
        {
            return await _context.BloodTypes
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        // GET: api/BloodTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BloodType>> GetBloodType(int id)
        {
            var bloodType = await _context.BloodTypes.FindAsync(id);

            if (bloodType == null)
                return NotFound();

            return bloodType;
        }

        // POST: api/BloodTypes
        [HttpPost]
        public async Task<ActionResult<BloodType>> PostBloodType(BloodType bloodType)
        {
            _context.BloodTypes.Add(bloodType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBloodType),
                new { id = bloodType.BloodTypeId },
                bloodType);
        }

        // PUT: api/BloodTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBloodType(int id, BloodType bloodType)
        {
            if (id != bloodType.BloodTypeId)
                return BadRequest();

            _context.Entry(bloodType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.BloodTypes.AnyAsync(x => x.BloodTypeId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/BloodTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBloodType(int id)
        {
            var bloodType = await _context.BloodTypes.FindAsync(id);

            if (bloodType == null)
                return NotFound();

            _context.BloodTypes.Remove(bloodType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}