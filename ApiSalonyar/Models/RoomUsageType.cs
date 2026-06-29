using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class RoomUsageType
    {
        public RoomUsageType()
        {
            Rooms = new HashSet<Room>();
        }

        public int RoomUsageTypeId { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
