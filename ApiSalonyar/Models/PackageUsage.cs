using System;
namespace ApiSalonyar.Models
{
    public partial class PackageUsage
    {
        public int PackageUsageId { get; set; }
        public int PatientPackageId { get; set; }
        public int? VisitId { get; set; }
        public int? ReservationId { get; set; }
        public int? SessionNumber { get; set; }
        public decimal? CreditUsed { get; set; }
        public DateTime UsedAt { get; set; }
        public int? UsedByStaffId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        // در ApiSalonyar.Models — به PackageUsage.cs اضافه کن
        public DateTime? SessionDate { get; set; }
        public TimeSpan? SessionTime { get; set; }
        public int? TreatmentId { get; set; }




        public virtual PatientPackage? PatientPackage { get; set; }
        public virtual PatientVisit? Visit { get; set; }
        public virtual RoomReservation? Reservation { get; set; }
        public virtual staff? UsedByStaff { get; set; }
    }
}