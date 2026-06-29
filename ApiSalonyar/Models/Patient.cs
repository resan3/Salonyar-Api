using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class Patient
    {
        public Patient()
        {
            PatientAllergies = new HashSet<PatientAllergy>();
            PatientConsentForms = new HashSet<PatientConsentForm>();
            PatientDiseases = new HashSet<PatientDisease>();
            PatientVisits = new HashSet<PatientVisit>();
            RoomReservations = new HashSet<RoomReservation>();
        }

        public int PatientId { get; set; }
        public int BranchId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string NationalCode { get; set; } = null!;
        public string Mobile { get; set; } = null!;
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? GenderId { get; set; }
        public DateTime? ReferralDate { get; set; }
        public int? ReferralSourceId { get; set; }
        public int? MaritalStatusId { get; set; }
        public int? ChildrenCount { get; set; }
        public string? ProfileImagePath { get; set; }
        public int? BloodTypeId { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual BloodType? BloodType { get; set; }
        public virtual Branch Branch { get; set; } = null!;
        public virtual Gender? Gender { get; set; }
        public virtual MaritalStatus? MaritalStatus { get; set; }
        public virtual ReferralSource? ReferralSource { get; set; }
        public virtual ICollection<PatientAllergy> PatientAllergies { get; set; }
        public virtual ICollection<PatientConsentForm> PatientConsentForms { get; set; }
        public virtual ICollection<PatientDisease> PatientDiseases { get; set; }
        public virtual ICollection<PatientVisit> PatientVisits { get; set; }
        public virtual ICollection<RoomReservation> RoomReservations { get; set; }
    }
}
