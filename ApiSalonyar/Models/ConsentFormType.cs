using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class ConsentFormType
    {
        public ConsentFormType()
        {
            PatientConsentForms = new HashSet<PatientConsentForm>();
        }

        public int ConsentFormTypeId { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<PatientConsentForm> PatientConsentForms { get; set; }
    }
}
