using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class TreatmentGroup
    {
        public TreatmentGroup()
        {
            Treatments = new HashSet<Treatment>();
        }

        public int TreatmentGroupId { get; set; }
        public string Title { get; set; } = null!;
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Treatment> Treatments { get; set; }
    }
}
