using System;
using System.Collections.Generic;
namespace ApiSalonyar.Models
{
    public partial class PatientPackage
    {
        public PatientPackage()
        {
            PackageUsages = new HashSet<PackageUsage>();
        }
        public int PatientPackageId { get; set; }
        public int BranchId { get; set; }
        public int PatientId { get; set; }
        public int PackageTemplateId { get; set; }
        public string? GiftCode { get; set; }
        public string? GiftRecipientName { get; set; }
        public string? GiftRecipientMobile { get; set; }
        public bool IsGiftUsed { get; set; }
        public int? GiftUsedByPatientId { get; set; }
        public int? TotalSessions { get; set; }
        public int UsedSessions { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal UsedCredit { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public string? Notes { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Branch? Branch { get; set; }
        public virtual Patient? Patient { get; set; }
        public virtual Patient? GiftUsedByPatient { get; set; }
        public virtual PackageTemplate? PackageTemplate { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public virtual ICollection<PackageUsage>? PackageUsages { get; set; }
    }
}