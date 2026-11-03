using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiSalonyar.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        [HttpGet("download")]
        public IActionResult Download()
        {
            var path = @"C:\BackupDB\ClinicDB.bak";
            if (!System.IO.File.Exists(path))
                return NotFound("فایل بک‌آپ موجود نیست");

            var cal = new System.Globalization.PersianCalendar();
            var now = DateTime.Now;
            var shamsi = $"{cal.GetYear(now)}{cal.GetMonth(now):D2}{cal.GetDayOfMonth(now):D2}";

            var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            return File(stream, "application/octet-stream", $"ClinicDB_{shamsi}.bak");
        }
    }
}
