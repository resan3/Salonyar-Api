using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ApiSalonyar.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public BranchesController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/Branches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches()
        {
            return await _context.Branches
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        // GET: api/Branches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);

            if (branch == null)
                return NotFound();

            return branch;
        }

        // POST: api/Branches
        [HttpPost]
        public async Task<ActionResult<Branch>> PostBranch(Branch branch)
        {
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBranch),
                new { id = branch.BranchId },
                branch);
        }

        // PUT: api/Branches/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(int id, Branch branch)
        {
            if (id != branch.BranchId)
                return BadRequest();

            _context.Entry(branch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Branches.AnyAsync(x => x.BranchId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Branches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);

            if (branch == null)
                return NotFound();

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}