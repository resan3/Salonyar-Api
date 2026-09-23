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


        [HttpGet]
        public async Task<IActionResult> GetVisits([FromQuery] int? patientId)
        {
            var query = _context.PatientVisits.AsQueryable();

            if (patientId.HasValue)
            {
                query = query.Where(v => v.PatientId == patientId.Value);
            }

            var visits = await query
                .Include(v => v.Patient)
                .Include(v => v.Staff)
                .Include(v => v.Treatment)
                .Select(v => new {
                    v.VisitId,
                    v.PatientId,
                    v.StaffId,
                    v.TreatmentId,
                    v.VisitDate,
                    v.VisitTime,
                    v.Notes,
                    v.BranchId,
                    Patient = v.Patient == null ? null : new { v.Patient.PatientId, v.Patient.FirstName, v.Patient.LastName, v.Patient.Mobile },
                    Staff = v.Staff == null ? null : new { v.Staff.StaffId, v.Staff.FullName },
                    Treatment = v.Treatment == null ? null : new { v.Treatment.TreatmentId, v.Treatment.Title }
                })
                .ToListAsync();

            return Ok(visits);
        }

        // GET: api/PatientVisits?patientId=5
        /*        [HttpGet]
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
                }*/

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
            // ✅ چک کن قبلاً برای این رزرو پرونده ثبت شده یا نه
            if (item.ReservationId.HasValue)
            {
                var exists = await _context.PatientVisits
                    .AnyAsync(x => !x.IsDeleted &&
                              x.ReservationId == item.ReservationId);
                if (exists)
                    return BadRequest("برای این رزرو قبلاً پرونده ثبت شده است.");
            }
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