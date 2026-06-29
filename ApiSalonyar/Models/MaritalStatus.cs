using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class MaritalStatus
    {
        public MaritalStatus()
        {
            Patients = new HashSet<Patient>();
        }

        public int MaritalStatusId { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
