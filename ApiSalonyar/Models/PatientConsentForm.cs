using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class PatientConsentForm
    {
        public int FormId { get; set; }
        public int PatientId { get; set; }
        public int ConsentFormTypeId { get; set; }
        public string FilePath { get; set; } = null!;
        public DateTime SignedDate { get; set; }
        public int? UploadedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ConsentFormType ConsentFormType { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;
        public virtual User? UploadedByUser { get; set; }
    }
}
