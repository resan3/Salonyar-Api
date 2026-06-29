using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class BloodType
    {
        public BloodType()
        {
            Patients = new HashSet<Patient>();
        }

        public int BloodTypeId { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
