using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientPackagesController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public PatientPackagesController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientPackage>>> GetPackages(
            [FromQuery] int? patientId, [FromQuery] string? status)
        {
            var query = _context.PatientPackages
                .Include(x => x.Patient)
                .Include(x => x.PackageTemplate).ThenInclude(x => x.Treatment)
                .Include(x => x.PackageUsages)
                .Where(x => !x.IsDeleted);

            if (patientId.HasValue) query = query.Where(x => x.PatientId == patientId.Value);
            if (!string.IsNullOrEmpty(status)) query = query.Where(x => x.Status == status);

            return await query.OrderByDescending(x => x.PurchaseDate).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientPackage>> GetPackage(int id)
        {
            var item = await _context.PatientPackages
                .Include(x => x.Patient)
                .Include(x => x.PackageTemplate).ThenInclude(x => x.Treatment)
                .Include(x => x.PackageUsages).ThenInclude(x => x.Visit)
                .FirstOrDefaultAsync(x => x.PatientPackageId == id && !x.IsDeleted);
            if (item == null) return NotFound();
            return item;
        }

        // ── خرید پکیج + صدور فاکتور خودکار ──
        [HttpPost]
        public async Task<ActionResult<object>> PostPackage(PatientPackage item)
        {
            ModelState.Clear();
            item.IsDeleted = false;
            item.CreatedAt = DateTime.Now;
            item.PurchaseDate = DateTime.Now;
            item.UsedSessions = 0;
            item.UsedCredit = 0;
            item.Status = "ACTIVE";
            item.BranchId = item.BranchId == 0 ? 1 : item.BranchId;

            var template = await _context.PackageTemplates.FindAsync(item.PackageTemplateId);
            if (template == null) return BadRequest("قالب پکیج یافت نشد.");

            item.TotalSessions = template.SessionCount;
            item.CreditAmount = template.CreditAmount;
            item.ExpiryDate = DateTime.Now.AddDays(template.ValidityDays);

            _context.PatientPackages.Add(item);
            await _context.SaveChangesAsync(); // ID گرفته می‌شه

            // مبلغ فاکتور:
            // جلسه‌ای → مبلغ پرداختی (قیمت پکیج)
            // اعتباری → مبلغ اعتبار (چون فاکتور باید با اعتبار همخوانی داشته باشه)
            bool isCredit = template.PackageType == "CREDIT";
            decimal invAmount = isCredit
                ? (template.CreditAmount ?? item.PaidAmount)
                : item.PaidAmount;

            var today = DateTime.Now;
            var count = await _context.Invoices.CountAsync(x => x.InvoiceDate.Date == today.Date);

            var invoice = new Invoice
            {
                BranchId = 1,
                PatientId = item.PatientId,
                InvoiceNumber = $"PKG-{today:yyyyMMdd}-{(count + 1):D3}",
                InvoiceDate = today,
                TotalAmount = invAmount,
                DiscountPercent = 0,
                DiscountAmount = 0,
                TaxPercent = 0,
                TaxAmount = 0,
                FinalAmount = invAmount,
                PaidAmount = 0,
                RemainingAmount = invAmount,
                Status = "PENDING",
                Notes = $"PKG:{item.PatientPackageId} - {template.Title}",
                CreatedAt = today,
                IsDeleted = false,
                InvoiceItems = new List<InvoiceItem>
                {
                    new InvoiceItem
                    {
                        Description     = template.Title,
                        Quantity        = 1,
                        UnitPrice       = invAmount,
                        DiscountPercent = 0,
                        DiscountAmount  = 0,
                        TotalPrice      = invAmount,
                    }
                }
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return Ok(new { patientPackage = item, invoice });
        }

        // ── استفاده از جلسه یا اعتبار ──
        [HttpPost("{id}/use")]
        public async Task<ActionResult<object>> UsePackage(int id, [FromBody] PackageUsage usage)
        {
            ModelState.Clear();
            var pkg = await _context.PatientPackages
                .Include(x => x.PackageTemplate)
                .FirstOrDefaultAsync(x => x.PatientPackageId == id && !x.IsDeleted);

            if (pkg == null) return NotFound();
            if (pkg.Status != "ACTIVE") return BadRequest("این پکیج فعال نیست.");
            if (pkg.ExpiryDate < DateTime.Now)
            {
                pkg.Status = "EXPIRED";
                await _context.SaveChangesAsync();
                return BadRequest("اعتبار این پکیج منقضی شده است.");
            }

            // چک جلسات (SESSION)
            if (pkg.TotalSessions.HasValue && pkg.UsedSessions >= pkg.TotalSessions)
                return BadRequest("تمام جلسات این پکیج استفاده شده است.");

            // چک اعتبار (CREDIT)
            if (pkg.CreditAmount.HasValue)
            {
                if (pkg.UsedCredit >= pkg.CreditAmount)
                    return BadRequest("اعتبار این پکیج تمام شده است.");
                if (usage.CreditUsed.HasValue &&
                    usage.CreditUsed.Value > (pkg.CreditAmount - pkg.UsedCredit))
                    return BadRequest("مبلغ درخواستی از اعتبار باقیمانده بیشتر است.");
            }

            usage.PatientPackageId = id;
            usage.UsedAt = DateTime.Now;
            usage.CreatedAt = DateTime.Now;
            usage.SessionNumber = pkg.UsedSessions + 1;

            _context.PackageUsages.Add(usage);

            pkg.UsedSessions += 1;
            if (usage.CreditUsed.HasValue) pkg.UsedCredit += usage.CreditUsed.Value;

            if (pkg.TotalSessions.HasValue && pkg.UsedSessions >= pkg.TotalSessions)
                pkg.Status = "COMPLETED";
            if (pkg.CreditAmount.HasValue && pkg.UsedCredit >= pkg.CreditAmount)
                pkg.Status = "COMPLETED";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                remainingSessions = pkg.TotalSessions.HasValue
                    ? pkg.TotalSessions - pkg.UsedSessions : (int?)null,
                remainingCredit = pkg.CreditAmount.HasValue
                    ? pkg.CreditAmount - pkg.UsedCredit : (decimal?)null,
                status = pkg.Status,
            });
        }

        // ── حذف پکیج ──
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var pkg = await _context.PatientPackages
                .FirstOrDefaultAsync(x => x.PatientPackageId == id && !x.IsDeleted);
            if (pkg == null) return NotFound();

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(x => x.Notes != null
                    && x.Notes.StartsWith($"PKG:{id} -") && !x.IsDeleted);

            if (invoice != null && invoice.PaidAmount > 0)
                return BadRequest("این پکیج پرداختی دارد. ابتدا از بخش حسابداری پرداختی‌ها را حذف کنید.");

            if (invoice != null) invoice.IsDeleted = true;
            pkg.IsDeleted = true;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // ── تغییر مبلغ اعتبار (جایگزینی) + آپدیت فاکتور ──
        [HttpPost("{id}/add-credit")]
        public async Task<IActionResult> AddCredit(int id, [FromBody] decimal amount)
        {
            var pkg = await _context.PatientPackages
                .FirstOrDefaultAsync(x => x.PatientPackageId == id && !x.IsDeleted);
            if (pkg == null) return NotFound();

            var oldCredit = pkg.CreditAmount ?? 0;
            var diff = amount - oldCredit; // می‌تونه منفی باشه

            pkg.CreditAmount = amount; // جایگزینی (نه اضافه)

            if (pkg.Status == "COMPLETED" && amount > pkg.UsedCredit)
                pkg.Status = "ACTIVE";

            // آپدیت فاکتور مرتبط
            var invoice = await _context.Invoices
                .Include(x => x.InvoiceItems)
                .FirstOrDefaultAsync(x => x.Notes != null
                    && x.Notes.StartsWith($"PKG:{id} -") && !x.IsDeleted);

            if (invoice != null && diff != 0)
            {
                invoice.TotalAmount += diff;
                invoice.FinalAmount += diff;
                invoice.RemainingAmount += diff;

                var invItem = invoice.InvoiceItems?.FirstOrDefault();
                if (invItem != null)
                {
                    invItem.UnitPrice += diff;
                    invItem.TotalPrice += diff;
                }

                invoice.Status = invoice.RemainingAmount <= 0 ? "PAID"
                    : invoice.PaidAmount > 0 ? "PARTIAL"
                    : "PENDING";
            }

            await _context.SaveChangesAsync();
            return Ok(new { newCredit = pkg.CreditAmount });
        }

        // ── چک انقضا ──
        [HttpPatch("check-expiry")]
        public async Task<IActionResult> CheckExpiry()
        {
            var expired = await _context.PatientPackages
                .Where(x => x.Status == "ACTIVE" && x.ExpiryDate < DateTime.Now && !x.IsDeleted)
                .ToListAsync();
            foreach (var p in expired) p.Status = "EXPIRED";
            await _context.SaveChangesAsync();
            return Ok(new { expiredCount = expired.Count });
        }
    }
}