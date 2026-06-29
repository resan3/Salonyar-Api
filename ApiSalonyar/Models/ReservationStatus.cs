using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class ReservationStatus
    {
        public ReservationStatus()
        {
            RoomReservations = new HashSet<RoomReservation>();
        }

        public int ReservationStatusId { get; set; }
        public string Title { get; set; } = null!;
        public string? ColorHex { get; set; }

        public virtual ICollection<RoomReservation> RoomReservations { get; set; }
    }
}
