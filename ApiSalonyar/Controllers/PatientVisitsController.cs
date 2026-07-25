using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientVisitsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public PatientVisitsController(ClinicDbContext context) => _context = context;

        // GET: api/PatientVisits?patientId=5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientVisit>>> GetVisits(
            [FromQuery] int? patientId)
        {
            var query = _context.PatientVisits
                .Include(x => x.Patient)
                .Include(x => x.Staff)
                .Include(x => x.Treatment)
                .Include(x => x.PatientImages)
                .Where(x => !x.IsDeleted);

            if (patientId.HasValue)
                query = query.Where(x => x.PatientId == patientId.Value);

            return await query
                .OrderByDescending(x => x.VisitDate)
                .ThenByDescending(x => x.VisitTime)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientVisit>> GetVisit(int id)
        {
            var item = await _context.PatientVisits
                .Include(x => x.Patient)
                .Include(x => x.Staff)
                .Include(x => x.Treatment)
                .Include(x => x.PatientImages)
                .FirstOrDefaultAsync(x => x.VisitId == id && !x.IsDeleted);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<PatientVisit>> PostVisit(PatientVisit item)
        {
            ModelState.Clear();
            item.IsDeleted = false;
            item.CreatedAt = DateTime.Now;
            item.BranchId = item.BranchId == 0 ? 1 : item.BranchId;
            _context.PatientVisits.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVisit), new { id = item.VisitId }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVisit(int id, PatientVisit item)
        {
            ModelState.Clear();
            var existing = await _context.PatientVisits.FindAsync(id);
            if (existing == null) return NotFound();

            existing.PatientId = item.PatientId;
            existing.StaffId = item.StaffId;
            existing.TreatmentId = item.TreatmentId;
            existing.VisitDate = item.VisitDate;
            existing.VisitTime = item.VisitTime;
            existing.Notes = item.Notes;
            existing.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisit(int id)
        {
            var item = await _context.PatientVisits.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}