using ApiSalonyar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientReportController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public PatientReportController(ClinicDbContext context) => _context = context;

        [HttpGet("{patientId}")]
        public async Task<ActionResult<object>> GetReport(int patientId)
        {
            try
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(x => x.PatientId == patientId && !x.IsDeleted);
                if (patient == null) return NotFound();

                // مراجعات
                var visits = await _context.PatientVisits
                    .Include(x => x.Treatment)
                    .Include(x => x.Staff)
                    .Where(x => x.PatientId == patientId && !x.IsDeleted)
                    .OrderByDescending(x => x.VisitDate)
                    .ToListAsync();

                // فاکتورها
                var invoices = await _context.Invoices
                    .Include(x => x.InvoiceItems).ThenInclude(x => x.Treatment)
                    .Include(x => x.Payments).ThenInclude(x => x.PaymentMethod)
                    .Where(x => x.PatientId == patientId && !x.IsDeleted)
                    .OrderByDescending(x => x.InvoiceDate)
                    .ToListAsync();

                // پکیج‌ها
                var packages = await _context.PatientPackages
                    .Include(x => x.PackageTemplate)
                    .Where(x => x.PatientId == patientId && !x.IsDeleted)
                    .ToListAsync();

                var totalInvoiced = invoices.Sum(x => x.FinalAmount);
                var totalPaid = invoices.Sum(x => x.PaidAmount);
                var totalDebt = invoices.Sum(x => x.RemainingAmount);

                var allPayments = invoices.SelectMany(x => x.Payments.Where(p => !p.IsDeleted)).ToList();
                var byMethod = allPayments
                    .GroupBy(x => x.PaymentMethod?.Title ?? "نامشخص")
                    .Select(g => new { method = g.Key, amount = g.Sum(x => x.Amount), count = g.Count() })
                    .ToList();

                return Ok(new
                {
                    patient = new { patient.PatientId, patient.FirstName, patient.LastName, patient.Mobile, patient.NationalCode, patient.CreatedAt },
                    summary = new { totalVisits = visits.Count, totalInvoiced, totalPaid, totalDebt, totalPackages = packages.Count },
                    byPaymentMethod = byMethod,
                    visits = visits.Select(v => new { v.VisitId, v.VisitDate, v.VisitTime, treatment = v.Treatment?.Title, staff = v.Staff?.FullName, v.Notes }),
                    invoices = invoices.Select(i => new { i.InvoiceId, i.InvoiceNumber, i.InvoiceDate, i.FinalAmount, i.PaidAmount, i.RemainingAmount, i.Status, payments = i.Payments.Where(p => !p.IsDeleted).Select(p => new { p.Amount, method = p.PaymentMethod?.Title, p.PaidAt }) }),
                    packages = packages.Select(p => new { p.PatientPackageId, title = p.PackageTemplate?.Title, p.TotalSessions, p.UsedSessions, p.CreditAmount, p.UsedCredit, p.Status, p.ExpiryDate, p.GiftCode }),
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + " | " + ex.InnerException?.Message);
            }
        }

    }

}
