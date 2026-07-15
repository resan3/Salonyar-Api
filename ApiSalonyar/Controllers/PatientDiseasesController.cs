using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientDiseasesController : ControllerBase
    {
        private readonly ClinicDbContext _context;

        public PatientDiseasesController(ClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/PatientDiseases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDisease>>> GetPatientDiseases()
        {
            return await _context.PatientDiseases
                .Include(x => x.Patient)
                .Include(x => x.Disease)
                .ToListAsync();
        }

        // GET: api/PatientDiseases/{patientId}/{diseaseId}
        [HttpGet("{patientId}/{diseaseId}")]
        public async Task<ActionResult<PatientDisease>> GetPatientDisease(int patientId, int diseaseId)
        {
            var item = await _context.PatientDiseases
                .Include(x => x.Patient)
                .Include(x => x.Disease)
                .FirstOrDefaultAsync(x => x.PatientId == patientId && x.DiseaseId == diseaseId);

            if (item == null)
                return NotFound();

            return item;
        }

        // POST: api/PatientDiseases
        [HttpPost]
        public async Task<ActionResult<PatientDisease>> PostPatientDisease(PatientDisease model)
        {
            _context.PatientDiseases.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // PUT: api/PatientDiseases/{patientId}/{diseaseId}
        [HttpPut("{patientId}/{diseaseId}")]
        public async Task<IActionResult> PutPatientDisease(int patientId, int diseaseId, PatientDisease model)
        {
            if (patientId != model.PatientId || diseaseId != model.DiseaseId)
                return BadRequest();

            var existing = await _context.PatientDiseases
                .FirstOrDefaultAsync(x => x.PatientId == patientId && x.DiseaseId == diseaseId);

            if (existing == null)
                return NotFound();

            existing.Note = model.Note;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PatientDiseases/{patientId}/{diseaseId}
        [HttpDelete("{patientId}/{diseaseId}")]
        public async Task<IActionResult> DeletePatientDisease(int patientId, int diseaseId)
        {
            var item = await _context.PatientDiseases
                .FirstOrDefaultAsync(x => x.PatientId == patientId && x.DiseaseId == diseaseId);

            if (item == null)
                return NotFound();

            _context.PatientDiseases.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}