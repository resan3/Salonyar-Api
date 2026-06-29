using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class UserPermission
    {
        public int UserId { get; set; }
        public int PermissionId { get; set; }
        public bool IsGranted { get; set; }

        public virtual Permission Permission { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
