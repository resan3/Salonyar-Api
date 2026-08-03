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
                .FirstOrDefaultAsync(x => x.Username == dto.Username && !x.IsDeleted && x.IsActive == true);

            if (user == null)
                return Unauthorized("نام کاربری یا رمز عبور اشتباه است.");

            // چک هش پسورد
            var hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(dto.Password)));

            if (user.PasswordHash != hash)
                return Unauthorized("نام کاربری یا رمز عبور اشتباه است.");

            return Ok(new
            {
                userId = user.UserId,
                username = user.Username,
                fullName = user.Staff?.FullName ?? user.Username,
                roles = user.Roles?.Select(r => r.Title).ToList(),
            });
        }
    }

    public class LoginDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
