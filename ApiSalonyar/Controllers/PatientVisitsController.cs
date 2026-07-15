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

        public PatientVisitsController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/PatientVisits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientVisit>>> GetPatientVisits()
        {
            return await _context.PatientVisits
                .Include(x => x.Patient)
                .Include(x => x.Branch)
                .Include(x => x.Staff)
                .Include(x => x.Treatment)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.VisitId)
                .ToListAsync();
        }

        // GET: api/PatientVisits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientVisit>> GetPatientVisit(int id)
        {
            var visit = await _context.PatientVisits
                .Include(x => x.Patient)
                .Include(x => x.Branch)
                .Include(x => x.Staff)
                .Include(x => x.Treatment)
                .Include(x => x.PatientImages)
                .FirstOrDefaultAsync(x => x.VisitId == id && !x.IsDeleted);

            if (visit == null)
                return NotFound();

            return visit;
        }

        // POST: api/PatientVisits
        [HttpPost]
        public async Task<ActionResult<PatientVisit>> PostPatientVisit(PatientVisit visit)
        {
            visit.CreatedAt = DateTime.Now;
            visit.IsDeleted = false;

            _context.PatientVisits.Add(visit);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatientVisit),
                new { id = visit.VisitId },
                visit);
        }

        // PUT: api/PatientVisits/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatientVisit(int id, PatientVisit visit)
        {
            if (id != visit.VisitId)
                return BadRequest();

            var existing = await _context.PatientVisits.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.PatientId = visit.PatientId;
            existing.BranchId = visit.BranchId;
            existing.TreatmentId = visit.TreatmentId;
            existing.StaffId = visit.StaffId;
            existing.VisitDate = visit.VisitDate;
            existing.VisitTime = visit.VisitTime;
            existing.Notes = visit.Notes;
            existing.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PatientVisits/5 (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientVisit(int id)
        {
            var visit = await _context.PatientVisits.FindAsync(id);

            if (visit == null)
                return NotFound();

            visit.IsDeleted = true;
            visit.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}