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
        public async Task<ActionResult> GetPatients()
        {
            var result = await _context.Patients
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.PatientId)
                .Select(x => new {
                    x.PatientId,
                    x.FirstName,
                    x.LastName,
                    x.NationalCode,
                    x.Mobile,
                    x.Phone,
                    x.BirthDate,
                    x.Address,
                    x.ReferralDate,
                    x.ReferralSourceId,
                    x.BranchId,
                    x.CreatedAt,
                    Gender = x.Gender == null ? null : new { x.Gender.GenderId, x.Gender.Title },
                    BloodType = x.BloodType == null ? null : new { x.BloodType.BloodTypeId, x.BloodType.Title },
                    MaritalStatus = x.MaritalStatus == null ? null : new { x.MaritalStatus.MaritalStatusId, x.MaritalStatus.Title },
                    PatientAllergies = x.PatientAllergies.Select(a => new { a.AllergyId }),
                    PatientDiseases = x.PatientDiseases.Select(d => new { d.DiseaseId })
                })
                .ToListAsync();

            return Ok(result);
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _context.Patients
                .Include(x => x.Branch)
                .Include(x => x.Gender)
                .Include(x => x.MaritalStatus)
                .Include(x => x.PatientAllergies)
                .Include(x => x.PatientDiseases)
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
            {
                string tempCode;
                do
                {
                    tempCode = DateTime.Now.Ticks.ToString().Substring(0, 15);
                } while (await _context.Patients.AnyAsync(x => x.NationalCode == tempCode && !x.IsDeleted));
                patient.NationalCode = tempCode;
            }
            else
            {
                // چک تکراری نبودن کد ملی وارد شده
                var exists = await _context.Patients
                    .AnyAsync(x => x.NationalCode == patient.NationalCode && !x.IsDeleted);
                if (exists)
                    return BadRequest("این کد ملی قبلاً ثبت شده است.");
            }

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

            // ✅ چک تکراری نبودن کد ملی
            var duplicateNc = await _context.Patients
                .AnyAsync(x => x.NationalCode == patient.NationalCode
                           && x.PatientId != id
                           && !x.IsDeleted);
            if (duplicateNc)
                return BadRequest("این کد ملی برای بیمار دیگری ثبت شده است.");

            // ✅ چک تکراری نبودن موبایل
            var duplicateMobile = await _context.Patients
                .AnyAsync(x => x.Mobile == patient.Mobile
                           && x.PatientId != id
                           && !x.IsDeleted);
            if (duplicateMobile)
                return BadRequest("این شماره موبایل برای بیمار دیگری ثبهت شده است.");

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