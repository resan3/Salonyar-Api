using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class Treatment
    {
        public Treatment()
        {
            PatientVisits = new HashSet<PatientVisit>();
            RoomReservations = new HashSet<RoomReservation>();
        }

        public int TreatmentId { get; set; }
        public int TreatmentGroupId { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual TreatmentGroup? TreatmentGroup { get; set; } = null!;
        public virtual ICollection<PatientVisit> PatientVisits { get; set; }
        public virtual ICollection<RoomReservation> RoomReservations { get; set; }
    }
}
