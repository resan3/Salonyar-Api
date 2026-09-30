using System;
using System.Collections.Generic;
namespace ApiSalonyar.Models
{
    public partial class PackageTemplate
    {
        public PackageTemplate()
        {
            PatientPackages = new HashSet<PatientPackage>();
        }
        public int PackageTemplateId { get; set; }
        public int BranchId { get; set; }
        public string Title { get; set; } = null!;
        public string PackageType { get; set; } = null!;
        public int? TreatmentId { get; set; }
        public int? SessionCount { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal Price { get; set; }
        public int ValidityDays { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Branch? Branch { get; set; }
        public virtual Treatment? Treatment { get; set; }
        public virtual ICollection<PatientPackage>? PatientPackages { get; set; }
    }
}