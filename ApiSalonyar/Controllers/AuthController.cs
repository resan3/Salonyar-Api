using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public AuthController(ClinicDbContext context) => _context = context;

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users
                .Include(x => x.Staff)
                .Include(x => x.Roles)
                    .ThenInclude(r => r.Permissions)
                .Include(x => x.UserPermissions)
                    .ThenInclude(x => x.Permission)
                .FirstOrDefaultAsync(x => x.Username == dto.Username && !x.IsDeleted && x.IsActive == true);

            if (user == null) return Unauthorized("نام کاربری یا رمز عبور اشتباه است.");

            var hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(dto.Password)));

            if (user.PasswordHash != hash)
                return Unauthorized("نام کاربری یا رمز عبور اشتباه است.");

            // ✅ جمع‌آوری همه دسترسی‌ها (از نقش‌ها + مستقیم)
            var permissions = new HashSet<string>();

            // دسترسی‌های نقش‌ها
            foreach (var role in user.Roles ?? new HashSet<Role>())
                foreach (var perm in role.Permissions ?? new HashSet<Permission>())
                    permissions.Add(perm.Code);

            // دسترسی‌های مستقیم (override)
            foreach (var up in user.UserPermissions ?? new HashSet<UserPermission>())
            {
                if (up.IsGranted) permissions.Add(up.Permission.Code);
                else permissions.Remove(up.Permission.Code); // سلب دسترسی
            }

            return Ok(new
            {
                userId = user.UserId,
                username = user.Username,
                staffId = user.StaffId,  // ✅ این رو اضافه کن

                fullName = user.Staff?.FullName ?? user.Username,
                roles = user.Roles?.Select(r => r.Title).ToList(),
                permissions = permissions.ToList(), // ✅ لیست کدهای دسترسی
            });
        }
    }

    public class LoginDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
