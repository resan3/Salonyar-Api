using ApiSalonyar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSalonyar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        public ReportsController(ClinicDbContext context) => _context = context;

        // ══════════════════════════════════════════════════════════════
        // GET /api/reports/staff?from=&to=&staffId=
        // گزارش عملکرد همکارها
        // ══════════════════════════════════════════════════════════════
        [HttpGet("staff")]
        public async Task<ActionResult> GetStaffReport(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int? staffId)
        {
            var query = _context.RoomReservations
                .Include(x => x.Staff)
                .Where(x => !x.IsDeleted && x.StaffId > 0);

            if (from.HasValue)
                query = query.Where(x => x.ReservationDate >= from.Value.Date);
            if (to.HasValue)
                query = query.Where(x => x.ReservationDate < to.Value.Date.AddDays(1));
            if (staffId.HasValue)
                query = query.Where(x => x.StaffId == staffId.Value);

            var reservations = await query.ToListAsync();

            var resIds = reservations.Select(x => x.ReservationId).ToList();
            var invoicePaidMap = await _context.Invoices
                .Where(x => !x.IsDeleted
                         && x.ReservationId.HasValue
                         && resIds.Contains(x.ReservationId!.Value))
                .ToDictionaryAsync(
                    x => x.ReservationId!.Value,
                    x => x.PaidAmount);

            var grouped = reservations
                .GroupBy(x => x.StaffId)
                .Select(g =>
                {
                    var sessions  = g.ToList();
                    var staffInfo = g.First().Staff;
                    return new
                    {
                        staffId        = g.Key,
                        staffName      = staffInfo != null ? staffInfo.FullName : "نامشخص",
                        sessionCount   = sessions.Count(),
                        totalMinutes   = sessions.Sum(r => CalcMinutes(r.StartTime, r.EndTime)),
                        totalAmount    = sessions
                                            .Where(r => invoicePaidMap.ContainsKey(r.ReservationId))
                                            .Sum(r => invoicePaidMap[r.ReservationId]),
                        attendanceDays = sessions
                                            .Select(r => r.ReservationDate.Date)
                                            .Distinct()
                                            .Count()
                    };
                })
                .OrderBy(x => x.staffName)
                .ToList();

            return Ok(grouped);
        }

        // ══════════════════════════════════════════════════════════════
        // GET /api/reports/ledger?from=&to=&patientId=
        // دفتر حساب — بدهکار / بستانکار / باقیمانده
        // منطق: فاکتور صادر شد → بستانکار (مشتری بدهکار ما میشه)
        //        پرداخت رسید     → بدهکار  (تسویه میشه)
        // ══════════════════════════════════════════════════════════════
        [HttpGet("ledger")]
        public async Task<ActionResult> GetLedger(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int? patientId)
        {
            // ── فاکتورها ─────────────────────────────────────────────
            var invQuery = _context.Invoices
                .Include(x => x.Patient)
                .Where(x => !x.IsDeleted && x.Status != "CANCELLED");

            if (from.HasValue)     invQuery = invQuery.Where(x => x.InvoiceDate >= from.Value.Date);
            if (to.HasValue)       invQuery = invQuery.Where(x => x.InvoiceDate < to.Value.Date.AddDays(1));
            if (patientId.HasValue) invQuery = invQuery.Where(x => x.PatientId == patientId.Value);

            var invoices = await invQuery.OrderBy(x => x.InvoiceDate).ToListAsync();

            // ── پرداخت‌های مرتبط ─────────────────────────────────────
            var invoiceIds = invoices.Select(x => x.InvoiceId).ToList();
            var payments   = await _context.Payments
                .Include(x => x.Invoice).ThenInclude(x => x.Patient)
                .Include(x => x.PaymentMethod)
                .Where(x => !x.IsDeleted && invoiceIds.Contains(x.InvoiceId))
                .OrderBy(x => x.PaidAt)
                .ToListAsync();

            // ── ساخت ردیف‌های دفتر ───────────────────────────────────
            var rows = new List<LedgerRow>();

            foreach (var inv in invoices)
            {
                var pName = inv.Patient != null
                    ? $"{inv.Patient.FirstName} {inv.Patient.LastName}"
                    : "نامشخص";

                rows.Add(new LedgerRow
                {
                    SortKey        = inv.InvoiceDate,
                    EntryDate      = inv.InvoiceDate.ToString("yyyy-MM-dd"),
                    EntryTime      = inv.InvoiceDate.ToString("HH:mm"),
                    DocumentNumber = inv.InvoiceNumber,
                    Description    = $"فاکتور فروش شماره {inv.InvoiceNumber} — {pName}",
                    Debit          = 0,
                    Credit         = inv.FinalAmount,
                    PatientId      = inv.PatientId,
                    PatientName    = pName,
                    Type           = "invoice",
                    RefId          = inv.InvoiceId,
                });
            }

            foreach (var pay in payments)
            {
                var pName = pay.Invoice?.Patient != null
                    ? $"{pay.Invoice.Patient.FirstName} {pay.Invoice.Patient.LastName}"
                    : "نامشخص";

                var pmTitle = pay.PaymentMethod?.Title ?? "";
                var codeStr = !string.IsNullOrEmpty(pay.ConfirmationCode)
                    ? $" — کد: {pay.ConfirmationCode}" : "";

                rows.Add(new LedgerRow
                {
                    SortKey        = pay.PaidAt,
                    EntryDate      = pay.PaidAt.ToString("yyyy-MM-dd"),
                    EntryTime      = pay.PaidAt.ToString("HH:mm"),
                    DocumentNumber = $"REC-{pay.PaymentId}",
                    Description    = $"دریافت بابت فاکتور {pay.Invoice?.InvoiceNumber} — {pmTitle}{codeStr}",
                    Debit          = pay.Amount,
                    Credit         = 0,
                    PatientId      = pay.Invoice?.PatientId ?? 0,
                    PatientName    = pName,
                    Type           = "payment",
                    RefId          = pay.PaymentId,
                });
            }

            // ── مرتب‌سازی و محاسبه باقیمانده ─────────────────────────
            var sorted  = rows.OrderBy(r => r.SortKey).ToList();
            decimal bal = 0;
            int     idx = 1;

            foreach (var row in sorted)
            {
                bal         += row.Credit - row.Debit;   // credit افزایش بدهی، debit کاهش
                row.Balance  = bal;
                row.Status   = bal > 0 ? "بستانکار" : "تسویه";
                row.RowIndex = idx++;
            }

            // ── خلاصه ────────────────────────────────────────────────
            var summary = new
            {
                totalCredit  = sorted.Sum(r => r.Credit),
                totalDebit   = sorted.Sum(r => r.Debit),
                finalBalance = bal,
                rowCount     = sorted.Count,
            };

            return Ok(new { summary, rows = sorted });
        }

        // ── helpers ──────────────────────────────────────────────────
        private static int CalcMinutes(TimeSpan start, TimeSpan end)
        {
            if (end <= start) return 0;
            return (int)(end - start).TotalMinutes;
        }

        private class LedgerRow
        {
            public DateTime SortKey        { get; set; }
            public int      RowIndex       { get; set; }
            public string   EntryDate      { get; set; } = "";
            public string   EntryTime      { get; set; } = "";
            public string   DocumentNumber { get; set; } = "";
            public string   Description    { get; set; } = "";
            public decimal  Debit          { get; set; }
            public decimal  Credit         { get; set; }
            public decimal  Balance        { get; set; }
            public string   Status         { get; set; } = "";
            public int      PatientId      { get; set; }
            public string   PatientName    { get; set; } = "";
            public string   Type           { get; set; } = "";
            public int      RefId          { get; set; }
        }
    }
}
