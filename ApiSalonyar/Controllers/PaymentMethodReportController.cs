using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodReportController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public PaymentMethodReportController(ClinicDbContext context) => _context = context;

        // GET: api/PaymentMethodReport?from=2024-01-01&to=2024-12-31
        [HttpGet]
        public async Task<ActionResult> GetReport(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var query = _context.Payments
                .Include(p => p.PaymentMethod)
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Patient)
                .Where(p => !p.IsDeleted);

            if (from.HasValue)
                query = query.Where(p => p.PaidAt >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(p => p.PaidAt < to.Value.Date.AddDays(1));

            var payments = await query
                .OrderByDescending(p => p.PaidAt)
                .Select(p => new
                {
                    p.PaymentId,
                    p.Amount,
                    p.PaidAt,
                    p.Notes,
                    p.InvoiceId,
                    PaymentMethod = p.PaymentMethod == null ? null : new
                    {
                        p.PaymentMethod.PaymentMethodId,
                        p.PaymentMethod.Title,
                        FeePercent = p.PaymentMethod.CommissionRate
                    },
                    Patient = p.Invoice == null || p.Invoice.Patient == null ? null : new
                    {
                        p.Invoice.Patient.PatientId,
                        p.Invoice.Patient.FirstName,
                        p.Invoice.Patient.LastName,
                        p.Invoice.Patient.Mobile
                    }
                })
                .ToListAsync();

            // گروه‌بندی بر اساس روش پرداخت
            var grouped = payments
                .GroupBy(p => new
                {
                    MethodId = p.PaymentMethod?.PaymentMethodId ?? 0,
                    Title = p.PaymentMethod?.Title ?? "نامشخص",
                    FeePercent = p.PaymentMethod == null ? 0m : p.PaymentMethod.FeePercent
                })
                .Select(g => new
                {
                    PaymentMethodId = g.Key.MethodId,
                    Title = g.Key.Title,
                    FeePercent = g.Key.FeePercent,  // همان CommissionRate از مدل
                    TotalAmount = g.Sum(x => x.Amount),
                    CompanyShare = g.Sum(x => x.Amount) * (g.Key.FeePercent / 100m),
                    NetAmount = g.Sum(x => x.Amount) * (1 - g.Key.FeePercent / 100m),
                    TransactionCount = g.Count(),
                    Transactions = g.OrderByDescending(x => x.PaidAt).Select(x => new
                    {
                        x.PaymentId,
                        x.Amount,
                        x.PaidAt,
                        x.Notes,
                        x.InvoiceId,
                        x.Patient,
                        FeeAmount = x.Amount * (g.Key.FeePercent / 100m),
                        NetAmount = x.Amount * (1 - g.Key.FeePercent / 100m),
                    }).ToList()
                })
                .OrderByDescending(g => g.TotalAmount)
                .ToList();

            return Ok(new
            {
                from,
                to,
                summary = new
                {
                    TotalReceived = payments.Sum(p => p.Amount),
                    TotalCompanyShare = grouped.Sum(g => g.CompanyShare),
                    TotalNet = grouped.Sum(g => g.NetAmount),
                    TransactionCount = payments.Count
                },
                byMethod = grouped
            });
        }
    }
}