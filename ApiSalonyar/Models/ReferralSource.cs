using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class ReferralSource
    {
        public ReferralSource()
        {
            Patients = new HashSet<Patient>();
        }

        public int ReferralSourceId { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
