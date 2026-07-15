using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public StaffController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<staff>>> GetStaff()
            => await _context.staff
                .Include(x => x.Profession)
                .Include(x => x.Branch)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.StaffId)
                .ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<staff>> GetStaffById(int id)
        {
            var item = await _context.staff
                .Include(x => x.Profession)
                .Include(x => x.Branch)
                .FirstOrDefaultAsync(x => x.StaffId == id && !x.IsDeleted);

            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<staff>> PostStaff(staff item)
        {
            ModelState.Clear();
            item.CreatedAt = DateTime.Now;
            item.IsDeleted = false;
            item.IsActive = true;
            item.BranchId = item.BranchId == 0 ? 1 : item.BranchId;

            _context.staff.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStaffById), new { id = item.StaffId }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaff(int id, staff item)
        {
            ModelState.Clear();
            if (id != item.StaffId) return BadRequest();

            var existing = await _context.staff.FindAsync(id);
            if (existing == null) return NotFound();

            existing.FullName = item.FullName;
            existing.Mobile = item.Mobile;
            existing.Email = item.Email;
            existing.Address = item.Address;
            existing.ProfessionId = item.ProfessionId;
            existing.Degree = item.Degree;
            existing.NationalCode = item.NationalCode;
            existing.HireDate = item.HireDate;
            existing.BranchId = item.BranchId;
            existing.IsActive = item.IsActive;
            existing.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var item = await _context.staff.FindAsync(id);
            if (item == null) return NotFound();

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}