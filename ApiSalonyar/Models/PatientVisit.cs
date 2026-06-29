using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class PatientVisit
    {
        public PatientVisit()
        {
            PatientImages = new HashSet<PatientImage>();
            RoomReservations = new HashSet<RoomReservation>();
        }

        public int VisitId { get; set; }
        public int PatientId { get; set; }
        public int BranchId { get; set; }
        public int TreatmentId { get; set; }
        public int StaffId { get; set; }
        public DateTime VisitDate { get; set; }
        public TimeSpan VisitTime { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Branch Branch { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;
        public virtual staff Staff { get; set; } = null!;
        public virtual Treatment Treatment { get; set; } = null!;
        public virtual ICollection<PatientImage> PatientImages { get; set; }
        public virtual ICollection<RoomReservation> RoomReservations { get; set; }
    }
}
