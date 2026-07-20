using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public PatientsController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            return await _context.Patients
                .Include(x => x.Branch)
                .Include(x => x.Gender)
                .Include(x => x.MaritalStatus)
                .Include(x => x.PatientAllergies)  // ✅ اضافه کن
                .Include(x => x.PatientDiseases)   // ✅ اضافه کن
                .Include(x => x.BloodType)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.PatientId)
                .ToListAsync();
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _context.Patients
                .Include(x => x.Branch)
                .Include(x => x.Gender)
                .Include(x => x.MaritalStatus)
                       .Include(x => x.PatientAllergies)  // ✅ اضافه کن
        .Include(x => x.PatientDiseases)   // ✅ اضافه کن
                .Include(x => x.BloodType)
                .FirstOrDefaultAsync(x => x.PatientId == id && !x.IsDeleted);

            if (patient == null)
                return NotFound();

            return patient;
        }

        // POST: api/Patients
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {


            ModelState.Clear();
            patient.CreatedAt = DateTime.Now;
            patient.IsDeleted = false;
            patient.BranchId = patient.BranchId == 0 ? 1 : patient.BranchId;

            // کد ملی موقت یکتا
            if (string.IsNullOrEmpty(patient.NationalCode))
                patient.NationalCode = DateTime.Now.Ticks.ToString().Substring(0, 15);

            // چک تکراری نبودن کد ملی
            var exists = await _context.Patients
                .AnyAsync(x => x.NationalCode == patient.NationalCode && !x.IsDeleted);
            if (exists)
                patient.NationalCode = DateTime.Now.Ticks.ToString().Substring(0, 15);


            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient),
                new { id = patient.PatientId },
                patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient patient)
        {
            ModelState.Clear();
            if (id != patient.PatientId) return BadRequest();

            var existing = await _context.Patients
                .Include(x => x.PatientAllergies)
                .Include(x => x.PatientDiseases)
                .FirstOrDefaultAsync(x => x.PatientId == id);

            if (existing == null) return NotFound();

            // آپدیت فیلدهای اصلی
            existing.FirstName = patient.FirstName;
            existing.LastName = patient.LastName;
            existing.NationalCode = patient.NationalCode;
            existing.Mobile = patient.Mobile;
            existing.Phone = patient.Phone;
            existing.BirthDate = patient.BirthDate;
            existing.GenderId = patient.GenderId;
            existing.ReferralDate = patient.ReferralDate;
            existing.ReferralSourceId = patient.ReferralSourceId;
            existing.MaritalStatusId = patient.MaritalStatusId;
            existing.ChildrenCount = patient.ChildrenCount;
            existing.BloodTypeId = patient.BloodTypeId;
            existing.Address = patient.Address;
            existing.EmergencyContactName = patient.EmergencyContactName;
            existing.EmergencyContactPhone = patient.EmergencyContactPhone;
            existing.Notes = patient.Notes;
            existing.BranchId = patient.BranchId;
            existing.UpdatedAt = DateTime.Now;

            // ✅ حذف آلرژی‌های قدیمی و جایگزینی با جدید
            _context.PatientAllergies.RemoveRange(existing.PatientAllergies);
            if (patient.PatientAllergies != null)
                foreach (var a in patient.PatientAllergies)
                    existing.PatientAllergies.Add(new PatientAllergy
                    {
                        PatientId = id,
                        AllergyId = a.AllergyId
                    });

            // ✅ حذف بیماری‌های قدیمی و جایگزینی با جدید
            _context.PatientDiseases.RemoveRange(existing.PatientDiseases);
            if (patient.PatientDiseases != null)
                foreach (var d in patient.PatientDiseases)
                    existing.PatientDiseases.Add(new PatientDisease
                    {
                        PatientId = id,
                        DiseaseId = d.DiseaseId
                    });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Patients/5  (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound();

            patient.IsDeleted = true;
            patient.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}