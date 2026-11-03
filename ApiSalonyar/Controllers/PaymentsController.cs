using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public PaymentsController(ClinicDbContext context) => _context = context;

        [HttpPost]
        public async Task<ActionResult<Payment>> PostPayment(Payment item)
        {
            ModelState.Clear();
            item.IsDeleted = false;
            item.CreatedAt = DateTime.Now;
            item.PaidAt = DateTime.Now;

            _context.Payments.Add(item);
            await _context.SaveChangesAsync();

            // آپدیت مبلغ پرداخت شده فاکتور
            var invoice = await _context.Invoices.FindAsync(item.InvoiceId);
            if (invoice != null)
            {
                var totalPaid = await _context.Payments
                    .Where(x => x.InvoiceId == item.InvoiceId && !x.IsDeleted)
                    .SumAsync(x => x.Amount);

                invoice.PaidAmount = totalPaid;
                invoice.RemainingAmount = invoice.FinalAmount - totalPaid;
                invoice.Status = totalPaid >= invoice.FinalAmount ? "PAID"
                               : totalPaid > 0 ? "PARTIAL"
                               : "PENDING";
                invoice.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return Ok(item);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var item = await _context.Payments.FindAsync(id);
            if (item == null) return NotFound();

            item.IsDeleted = true;
            await _context.SaveChangesAsync();

            // ✅ آپدیت فاکتور بعد از حذف
            var invoice = await _context.Invoices.FindAsync(item.InvoiceId);
            if (invoice != null)
            {
                // فقط پرداخت‌های حذف نشده رو حساب کن
                var totalPaid = await _context.Payments
                    .Where(x => x.InvoiceId == item.InvoiceId && !x.IsDeleted)
                    .SumAsync(x => x.Amount);

                invoice.PaidAmount = totalPaid;
                invoice.RemainingAmount = invoice.FinalAmount - totalPaid;
                invoice.Status = totalPaid >= invoice.FinalAmount ? "PAID"
                               : totalPaid > 0 ? "PARTIAL"
                               : "PENDING";
                invoice.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> GetPayments(
    [FromQuery] string? notes,
    [FromQuery] int? paymentMethodId)
        {
            var query = _context.Payments
                .Include(x => x.Invoice).ThenInclude(x => x.Patient)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(notes))
                query = query.Where(x => x.Notes != null && x.Notes.Contains(notes));
            if (paymentMethodId.HasValue)
                query = query.Where(x => x.PaymentMethodId == paymentMethodId.Value);

            var result = await query.Select(x => new {
                x.PaymentId,
                x.Amount,
                x.PaymentMethodId,
                x.Notes,
                x.PaidAt,
                x.CreatedAt,
                Patient = x.Invoice == null || x.Invoice.Patient == null ? null : new
                {
                    x.Invoice.Patient.PatientId,
                    x.Invoice.Patient.FirstName,
                    x.Invoice.Patient.LastName,
                }
            }).ToListAsync();

            return Ok(result);
        }
    }

}
