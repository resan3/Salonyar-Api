using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public PermissionsController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Permission>>> GetPermissions()
            => await _context.Permissions
                .OrderBy(x => x.ModuleName)
                .ThenBy(x => x.Title)
                .ToListAsync();
    }
}
