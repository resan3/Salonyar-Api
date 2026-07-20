using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class Room
    {
        public Room()
        {
            RoomReservations = new HashSet<RoomReservation>();
        }

        public int RoomId { get; set; }
        public int BranchId { get; set; }
        public string Title { get; set; } = null!;
        public decimal? Area { get; set; }
        public int? RoomUsageTypeId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Branch? Branch { get; set; } = null!;
        public virtual RoomUsageType? RoomUsageType { get; set; }
        public virtual ICollection<RoomReservation>? RoomReservations { get; set; }
    }
}
