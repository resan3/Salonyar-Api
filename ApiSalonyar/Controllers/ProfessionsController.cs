using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public ProfessionsController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profession>>> GetProfessions()
            => await _context.Professions.ToListAsync();
    }
}
