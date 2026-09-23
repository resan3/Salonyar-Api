using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientImagesController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadPath;  // ✅ اضافه کن



            public PatientImagesController(ClinicDbContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _uploadPath = configuration["UploadPath"] ?? "D:\\ClinicUploads";  // ✅ اضافه کن

        }

        // GET: api/PatientImages?visitId=5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientImage>>> GetImages(
            [FromQuery] int? visitId)
        {
            var query = _context.PatientImages
                .Include(x => x.Visit)
                .Where(x => !x.IsDeleted);

            if (visitId.HasValue)
                query = query.Where(x => x.VisitId == visitId.Value);

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientImage>> GetImage(int id)
        {
            var item = await _context.PatientImages
                .Include(x => x.Visit)
                .FirstOrDefaultAsync(x => x.ImageId == id && !x.IsDeleted);
            if (item == null) return NotFound();
            return item;
        }

        // POST: api/PatientImages/upload
        // آپلود فایل + ذخیره آدرس
        [HttpPost("upload")]
        public async Task<ActionResult<object>> UploadImage(
            [FromForm] int visitId,
            [FromForm] string imageType,       // "before" یا "after"
            [FromForm] string? description,
            IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("فایلی انتخاب نشده.");

            // چک نوع فایل
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowed.Contains(ext))
                return BadRequest("فقط فایل‌های تصویری مجاز هستند.");

            // ساخت پوشه ذخیره‌سازی
            //var uploadPath = Path.Combine("D:\\ClinicUploads", "patients", visitId.ToString());
            // var uploadPath = Path.Combine("C:\\wwwroot\\heseno\\api.rayanakshop.ir\\wwwroot\\uploads", "patients", visitId.ToString());
            var uploadPath = Path.Combine(_uploadPath, "patients", visitId.ToString());
            Directory.CreateDirectory(uploadPath);

            // نام یکتا برای فایل
            var fileName = $"{imageType}_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString()[..8]}{ext}";
            var filePath = Path.Combine(uploadPath, fileName);

            // ذخیره فایل
            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            // آدرس نسبی برای ذخیره در دیتابیس
            var relativePath = $"/uploads/patients/{visitId}/{fileName}";

            // پیدا کردن یا ساخت رکورد PatientImage برای این Visit
            var existing = await _context.PatientImages
                .FirstOrDefaultAsync(x => x.VisitId == visitId && !x.IsDeleted);

            if (existing == null)
            {
                existing = new PatientImage
                {
                    VisitId = visitId,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false,
                    Description = description,
                };
                _context.PatientImages.Add(existing);
            }

            // ست کردن مسیر قبل یا بعد
            if (imageType == "before")
                existing.BeforeImagePath = relativePath;
            else
                existing.AfterImagePath = relativePath;

            if (!string.IsNullOrEmpty(description))
                existing.Description = description;

            await _context.SaveChangesAsync();

            return Ok(new { path = relativePath, imageId = existing.ImageId });
        }

        // DELETE: api/PatientImages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var item = await _context.PatientImages.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
