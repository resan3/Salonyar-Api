using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationTemp : ControllerBase
    {
        private readonly ClinicDbContext _context;

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


    }
}
