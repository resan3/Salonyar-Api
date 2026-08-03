using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public UsersController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
            => await _context.Users
                .Include(x => x.Staff)
                .Include(x => x.Roles)
                .Include(x => x.UserPermissions)
                    .ThenInclude(x => x.Permission)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.UserId)
                .ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(x => x.Staff)
                .Include(x => x.Roles)
                .Include(x => x.UserPermissions)
                    .ThenInclude(x => x.Permission)
                .FirstOrDefaultAsync(x => x.UserId == id && !x.IsDeleted);
            if (user == null) return NotFound();
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {
            ModelState.Clear();

            // ✅ اول roleIds رو بگیر قبل از هر چیز
            var roleIds = user.Roles?.Select(r => r.RoleId).ToList() ?? new List<int>();
            user.Roles = new HashSet<Role>(); // خالی کن تا validation نره روش

            user.PasswordHash = HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.Now;
            user.IsDeleted = false;
            user.IsActive = true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // بعد از ذخیره نقش‌ها رو اضافه کن
            foreach (var roleId in roleIds)
            {
                var role = await _context.Roles.FindAsync(roleId);
                if (role != null) user.Roles.Add(role);
            }
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            ModelState.Clear();
            var existing = await _context.Users
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.UserId == id);
            if (existing == null) return NotFound();

            existing.Username = user.Username;
            existing.StaffId = user.StaffId;
            existing.IsActive = user.IsActive;

            // تغییر پسورد فقط اگه ارسال شده
            if (!string.IsNullOrEmpty(user.PasswordHash))
                existing.PasswordHash = HashPassword(user.PasswordHash);

            // آپدیت نقش‌ها
            // آپدیت نقش‌ها
            var roleIds = user.Roles?.Select(r => r.RoleId).ToList() ?? new List<int>();
            existing.Roles.Clear();
            foreach (var roleId in roleIds)
            {
                var role = await _context.Roles.FindAsync(roleId);
                if (role != null) existing.Roles.Add(role);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // آپدیت دسترسی‌های یه کاربر
        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> UpdatePermissions(int id, [FromBody] List<UserPermission> permissions)
        {
            ModelState.Clear();
            var existing = await _context.UserPermissions
                .Where(x => x.UserId == id)
                .ToListAsync();
            _context.UserPermissions.RemoveRange(existing);

            foreach (var p in permissions)
            {
                p.UserId = id;
                _context.UserPermissions.Add(p);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

    }
}
