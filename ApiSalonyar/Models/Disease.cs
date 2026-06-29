using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class Disease
    {
        public Disease()
        {
            PatientDiseases = new HashSet<PatientDisease>();
        }

        public int DiseaseId { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<PatientDisease> PatientDiseases { get; set; }
    }
}
