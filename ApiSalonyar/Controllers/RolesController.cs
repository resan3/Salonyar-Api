using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiSalonyar.Models;
using Microsoft.EntityFrameworkCore;


namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public RolesController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
            => await _context.Roles
                .Include(x => x.Permissions)
                .ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            ModelState.Clear();
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return Ok(role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, Role role)
        {
            ModelState.Clear();
            var existing = await _context.Roles
                .Include(x => x.Permissions)
                .FirstOrDefaultAsync(x => x.RoleId == id);
            if (existing == null) return NotFound();

            existing.Title = role.Title;

            // آپدیت دسترسی‌های نقش
            existing.Permissions.Clear();
            if (role.Permissions != null)
                foreach (var p in role.Permissions)
                {
                    var perm = await _context.Permissions.FindAsync(p.PermissionId);
                    if (perm != null) existing.Permissions.Add(perm);
                }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
