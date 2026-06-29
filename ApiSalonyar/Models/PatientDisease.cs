using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class PatientDisease
    {
        public int PatientId { get; set; }
        public int DiseaseId { get; set; }
        public string? Note { get; set; }

        public virtual Disease Disease { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;
    }
}
