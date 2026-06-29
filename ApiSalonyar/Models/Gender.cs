using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class Gender
    {
        public Gender()
        {
            Patients = new HashSet<Patient>();
        }

        public int GenderId { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
