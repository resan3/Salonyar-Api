using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class PatientImage
    {
        public int ImageId { get; set; }
        public int VisitId { get; set; }
        public string? BeforeImagePath { get; set; }
        public string? AfterImagePath { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual PatientVisit Visit { get; set; } = null!;
    }
}
