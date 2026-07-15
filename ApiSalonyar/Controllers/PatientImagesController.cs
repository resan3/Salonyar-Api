using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientImagesController : ControllerBase
    {
/*        private readonly ClinicDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PatientImagesController(ClinicDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/PatientImages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientImage>>> GetImages()
        {
            return await _context.PatientImages
                .Include(x => x.Visit)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.ImageId)
                .ToListAsync();
        }

        // GET: api/PatientImages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientImage>> GetImage(int id)
        {
            var img = await _context.PatientImages
                .Include(x => x.Visit)
                .FirstOrDefaultAsync(x => x.ImageId == id && !x.IsDeleted);

            if (img == null)
                return NotFound();

            return img;
        }

        // POST: Upload Image (Before/After)
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(
            [FromForm] int visitId,
            [FromForm] IFormFile? beforeImage,
            [FromForm] IFormFile? afterImage,
            [FromForm] string? description)
        {
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string? beforePath = null;
            string? afterPath = null;

            if (beforeImage != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(beforeImage.FileName);
                var fullPath = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await beforeImage.CopyToAsync(stream);

                beforePath = "/uploads/" + fileName;
            }

            if (afterImage != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(afterImage.FileName);
                var fullPath = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await afterImage.CopyToAsync(stream);

                afterPath = "/uploads/" + fileName;
            }

            var entity = new PatientImage
            {
                VisitId = visitId,
                BeforeImagePath = beforePath,
                AfterImagePath = afterPath,
                Description = description,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.PatientImages.Add(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        // DELETE (soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var img = await _context.PatientImages.FindAsync(id);

            if (img == null)
                return NotFound();

            img.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }*/
    }
}