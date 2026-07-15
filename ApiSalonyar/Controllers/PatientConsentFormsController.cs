using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientConsentFormsController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public PatientConsentFormsController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/PatientConsentForms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientConsentForm>>> GetPatientConsentForms()
        {
            return await _context.PatientConsentForms
                .Include(x => x.Patient)
                .Include(x => x.ConsentFormType)
                .Include(x => x.UploadedByUser)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.FormId)
                .ToListAsync();
        }

        // GET: api/PatientConsentForms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientConsentForm>> GetPatientConsentForm(int id)
        {
            var form = await _context.PatientConsentForms
                .Include(x => x.Patient)
                .Include(x => x.ConsentFormType)
                .Include(x => x.UploadedByUser)
                .FirstOrDefaultAsync(x => x.FormId == id && !x.IsDeleted);

            if (form == null)
                return NotFound();

            return form;
        }

        // POST: api/PatientConsentForms
        [HttpPost]
        public async Task<ActionResult<PatientConsentForm>> PostPatientConsentForm(PatientConsentForm form)
        {
            form.CreatedAt = DateTime.Now;
            form.IsDeleted = false;

            _context.PatientConsentForms.Add(form);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatientConsentForm),
                new { id = form.FormId },
                form);
        }

        // PUT: api/PatientConsentForms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatientConsentForm(int id, PatientConsentForm form)
        {
            if (id != form.FormId)
                return BadRequest();

            var existing = await _context.PatientConsentForms.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.PatientId = form.PatientId;
            existing.ConsentFormTypeId = form.ConsentFormTypeId;
            existing.FilePath = form.FilePath;
            existing.SignedDate = form.SignedDate;
            existing.UploadedByUserId = form.UploadedByUserId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PatientConsentForms/5 (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientConsentForm(int id)
        {
            var form = await _context.PatientConsentForms.FindAsync(id);

            if (form == null)
                return NotFound();

            form.IsDeleted = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}