using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class Allergy
    {
        public Allergy()
        {
            PatientAllergies = new HashSet<PatientAllergy>();
        }

        public int AllergyId { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<PatientAllergy> PatientAllergies { get; set; }
    }
}
