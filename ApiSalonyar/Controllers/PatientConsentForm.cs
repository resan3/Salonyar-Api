using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientConsentFormsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public PatientConsentFormsController(ClinicDbContext context) => _context = context;

        // GET: api/PatientConsentForms?patientId=5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientConsentForm>>> GetForms(
            [FromQuery] int? patientId)
        {
            var query = _context.PatientConsentForms
                .Include(x => x.ConsentFormType)
                .Include(x => x.Patient)
                .Where(x => !x.IsDeleted);

            if (patientId.HasValue)
                query = query.Where(x => x.PatientId == patientId.Value);

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        // POST: api/PatientConsentForms/upload
        [HttpPost("upload")]
        public async Task<ActionResult<object>> Upload(
            [FromForm] int patientId,
            [FromForm] int consentFormTypeId,
            [FromForm] string signedDate,
            IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("فایلی انتخاب نشده.");

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".pdf")
                return BadRequest("فقط فایل PDF مجاز است.");

            // ساخت پوشه
            var uploadPath = Path.Combine("D:\\ClinicUploads", "consent", patientId.ToString());
            Directory.CreateDirectory(uploadPath);

            // نام یکتا
            var fileName = $"consent_{consentFormTypeId}_{DateTime.Now:yyyyMMdd_HHmmss}{ext}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var relativePath = $"/uploads/consent/{patientId}/{fileName}";

            var form = new PatientConsentForm
            {
                PatientId = patientId,
                ConsentFormTypeId = consentFormTypeId,
                FilePath = relativePath,
                SignedDate = DateTime.Parse(signedDate),
                CreatedAt = DateTime.Now,
                IsDeleted = false,
            };

            _context.PatientConsentForms.Add(form);
            await _context.SaveChangesAsync();

            return Ok(new { formId = form.FormId, filePath = relativePath });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(int id)
        {
            var item = await _context.PatientConsentForms.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
