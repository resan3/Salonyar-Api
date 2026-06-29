using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class User
    {
        public User()
        {
            PatientConsentForms = new HashSet<PatientConsentForm>();
            RoomReservations = new HashSet<RoomReservation>();
            UserPermissions = new HashSet<UserPermission>();
            Roles = new HashSet<Role>();
        }

        public int UserId { get; set; }
        public int? StaffId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual staff? Staff { get; set; }
        public virtual ICollection<PatientConsentForm> PatientConsentForms { get; set; }
        public virtual ICollection<RoomReservation> RoomReservations { get; set; }
        public virtual ICollection<UserPermission> UserPermissions { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
