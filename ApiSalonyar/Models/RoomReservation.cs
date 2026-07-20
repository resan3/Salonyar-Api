using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class RoomReservation
    {
        public int ReservationId { get; set; }
        public int BranchId { get; set; }
        public int PatientId { get; set; }
        public int StaffId { get; set; }
        public int RoomId { get; set; }
        public int? TreatmentId { get; set; }
        public int ReservationStatusId { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? VisitId { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // ✅ فقط ? - بدون = null!
        public virtual Branch? Branch { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public virtual Patient? Patient { get; set; }
        public virtual ReservationStatus? ReservationStatus { get; set; }
        public virtual Room? Room { get; set; }
        public virtual staff? Staff { get; set; }
        public virtual Treatment? Treatment { get; set; }
        public virtual PatientVisit? Visit { get; set; }
    }
}