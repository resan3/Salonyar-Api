using System;
namespace ApiSalonyar.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int StaffId { get; set; }
        public int? UserId { get; set; }
        public int? ReservationId { get; set; }
        public int? InvoiceId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Priority { get; set; } = "NORMAL";
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool ActionRequired { get; set; }
        public string? ActionType { get; set; }
        public DateTime? ActionDoneAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual staff?          Staff       { get; set; }
        public virtual User?           User        { get; set; }
        public virtual RoomReservation? Reservation { get; set; }
        public virtual Invoice?        Invoice     { get; set; }
    }
}
