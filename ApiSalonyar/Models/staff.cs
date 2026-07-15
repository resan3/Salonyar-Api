using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class staff
    {
        public staff()
        {
            PatientVisits = new HashSet<PatientVisit>();
            RoomReservations = new HashSet<RoomReservation>();
            Users = new HashSet<User>();
        }

        public int StaffId { get; set; }
        public int BranchId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? ProfessionId { get; set; }
        public string? Degree { get; set; }
        public string? NationalCode { get; set; }
        public DateTime? HireDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Branch? Branch { get; set; } = null!;
        public virtual Profession? Profession { get; set; }
        public virtual ICollection<PatientVisit> PatientVisits { get; set; }
        public virtual ICollection<RoomReservation> RoomReservations { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
