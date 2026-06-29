using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class Branch
    {
        public Branch()
        {
            PatientVisits = new HashSet<PatientVisit>();
            Patients = new HashSet<Patient>();
            RoomReservations = new HashSet<RoomReservation>();
            Rooms = new HashSet<Room>();
            staff = new HashSet<staff>();
        }

        public int BranchId { get; set; }
        public string Title { get; set; } = null!;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<PatientVisit> PatientVisits { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
        public virtual ICollection<RoomReservation> RoomReservations { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<staff> staff { get; set; }
    }
}
