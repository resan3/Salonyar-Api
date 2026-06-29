using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class Permission
    {
        public Permission()
        {
            UserPermissions = new HashSet<UserPermission>();
            Roles = new HashSet<Role>();
        }

        public int PermissionId { get; set; }
        public string Code { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string ModuleName { get; set; } = null!;

        public virtual ICollection<UserPermission> UserPermissions { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
