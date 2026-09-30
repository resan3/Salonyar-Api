using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicSettingsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public ClinicSettingsController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<ClinicSettings>> GetSettings()
        {
            var settings = await _context.ClinicSettings
                .FirstOrDefaultAsync(x => x.BranchId == 1);

            // اگه تنظیمات نبود، پیش‌فرض برگردون
            if (settings == null)
            {
                settings = new ClinicSettings
                {
                    BranchId = 1,
                    AutoCreateVisit = false,
                    AutoCreateInvoice = false,
                    AutoCreateInvoiceOnPackage = true,
                    CancelExpiredDays = 15,
                    AutoCancelExpired = false,
                    ShowPaymentInReservation = false,
                };
                _context.ClinicSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            return Ok(settings);
        }

        [HttpPut]
        public async Task<IActionResult> PutSettings(ClinicSettings item)
        {
            ModelState.Clear();
            var existing = await _context.ClinicSettings
                .FirstOrDefaultAsync(x => x.BranchId == 1);

            if (existing == null)
            {
                item.BranchId = 1;
                _context.ClinicSettings.Add(item);
            }
            else
            {
                existing.AutoCreateVisit = item.AutoCreateVisit;
                existing.AutoCreateInvoice = item.AutoCreateInvoice;
                existing.AutoCreateInvoiceOnPackage = item.AutoCreateInvoiceOnPackage;
                existing.CancelExpiredDays = item.CancelExpiredDays;
                existing.AutoCancelExpired = item.AutoCancelExpired;
                existing.ShowPaymentInReservation = item.ShowPaymentInReservation;
                existing.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
