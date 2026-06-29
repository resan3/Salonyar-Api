using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class PatientAllergy
    {
        public int PatientId { get; set; }
        public int AllergyId { get; set; }
        public string? Note { get; set; }

        public virtual Allergy Allergy { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;
    }
}
