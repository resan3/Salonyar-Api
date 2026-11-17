using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public InvoicesController(ClinicDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult> GetInvoices(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int? patientId,
            [FromQuery] string? status)
        {
            var query = _context.Invoices.Where(x => !x.IsDeleted);

            if (from.HasValue) query = query.Where(x => x.InvoiceDate >= from.Value);
            if (to.HasValue) query = query.Where(x => x.InvoiceDate <= to.Value.AddDays(1));
            if (patientId.HasValue) query = query.Where(x => x.PatientId == patientId.Value);
            if (!string.IsNullOrEmpty(status)) query = query.Where(x => x.Status == status);

            var result = await query
                .OrderByDescending(x => x.InvoiceDate)
                .Select(x => new {
                    x.InvoiceId,
                    x.InvoiceNumber,
                    x.InvoiceDate,
                    x.Status,
                    x.TotalAmount,
                    x.FinalAmount,
                    x.PaidAmount,
                    x.RemainingAmount,
                    x.DiscountAmount,
                    x.TaxAmount,
                    x.ReservationId,
                    x.TipAmount,
                    x.TipStaffId,
                    Patient = x.Patient == null ? null : new
                    {
                        x.Patient.PatientId,
                        x.Patient.FirstName,
                        x.Patient.LastName,
                        x.Patient.Mobile
                    }
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            var item = await _context.Invoices
                .Include(x => x.Patient)
                .Include(x => x.Reservation)
                .Include(x => x.InvoiceItems).ThenInclude(x => x.Treatment)
                .Include(x => x.Payments).ThenInclude(x => x.PaymentMethod)
                .Include(x => x.Payments).ThenInclude(x => x.CollectedByStaff)
                .Include(x => x.Refunds)
                .FirstOrDefaultAsync(x => x.InvoiceId == id && !x.IsDeleted);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice item)
        {
            ModelState.Clear();
            if (item.ReservationId.HasValue)
            {
                var exists = await _context.Invoices
                    .AnyAsync(x => !x.IsDeleted &&
                              x.ReservationId == item.ReservationId);
                if (exists)
                    return BadRequest("برای این رزرو قبلاً فاکتور ثبت شده است.");
            }
            item.IsDeleted = false;
            item.CreatedAt = DateTime.Now;
            item.BranchId = item.BranchId == 0 ? 1 : item.BranchId;
            item.InvoiceDate = DateTime.Now;
            item.Status = "PENDING";

            var today = DateTime.Now;
            var count = await _context.Invoices
                .CountAsync(x => x.InvoiceDate.Date == today.Date);
            item.InvoiceNumber = $"INV-{today:yyyyMMdd}-{(count + 1):D3}";

            item.TotalAmount = item.InvoiceItems?.Sum(x => x.TotalPrice) ?? 0;
            item.DiscountAmount = item.DiscountAmount > 0
                ? item.DiscountAmount
                : item.TotalAmount * item.DiscountPercent / 100;
            item.TaxAmount = (item.TotalAmount - item.DiscountAmount) * item.TaxPercent / 100;
            item.FinalAmount = item.TotalAmount - item.DiscountAmount + item.TaxAmount;
            item.PaidAmount = 0;
            item.RemainingAmount = item.FinalAmount;

            // انعام — فقط ذخیره می‌کنیم، به FinalAmount اضافه نمیشه
            item.TipAmount   = item.TipAmount > 0 ? item.TipAmount : 0;
            item.TipStaffId  = item.TipStaffId > 0 ? item.TipStaffId : null;

            _context.Invoices.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice item)
        {
            ModelState.Clear();
            var existing = await _context.Invoices
                .Include(x => x.InvoiceItems)
                .FirstOrDefaultAsync(x => x.InvoiceId == id);
            if (existing == null) return NotFound();

            existing.Notes = item.Notes;
            existing.DiscountPercent = item.DiscountPercent;
            existing.TaxPercent = item.TaxPercent;
            existing.UpdatedAt = DateTime.Now;

            // ✅ انعام
            existing.TipAmount  = item.TipAmount > 0 ? item.TipAmount : 0;
            existing.TipStaffId = item.TipStaffId > 0 ? item.TipStaffId : null;

            _context.InvoiceItems.RemoveRange(existing.InvoiceItems ?? new List<InvoiceItem>());
            if (item.InvoiceItems != null)
                foreach (var i in item.InvoiceItems)
                {
                    i.InvoiceId = id;
                    _context.InvoiceItems.Add(i);
                }

            existing.TotalAmount = item.InvoiceItems?.Sum(x => x.TotalPrice) ?? 0;
            existing.DiscountAmount = item.DiscountAmount > 0
                ? item.DiscountAmount
                : existing.TotalAmount * existing.DiscountPercent / 100;
            existing.TaxAmount = (existing.TotalAmount - existing.DiscountAmount) * existing.TaxPercent / 100;
            existing.FinalAmount = existing.TotalAmount - existing.DiscountAmount + existing.TaxAmount;
            existing.RemainingAmount = existing.FinalAmount - existing.PaidAmount;

            existing.Status = existing.PaidAmount >= existing.FinalAmount && existing.FinalAmount > 0 ? "PAID"
                            : existing.PaidAmount > 0 ? "PARTIAL"
                            : "PENDING";

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var item = await _context.Invoices.FindAsync(id);
            if (item == null) return NotFound();
            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
