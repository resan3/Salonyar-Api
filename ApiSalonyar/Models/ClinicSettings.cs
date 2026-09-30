using System;
namespace ApiSalonyar.Models
{
    public partial class ClinicSettings
    {
        public int SettingId { get; set; }
        public int BranchId { get; set; }
        public bool AutoCreateVisit { get; set; }
        public bool AutoCreateInvoice { get; set; }
        public bool AutoCreateInvoiceOnPackage { get; set; }
        public int CancelExpiredDays { get; set; }
        public bool AutoCancelExpired { get; set; }
        public bool ShowPaymentInReservation { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; }

        public virtual Branch? Branch { get; set; }
        public virtual User? UpdatedByUser { get; set; }
    }
}