using System;
using System.Collections.Generic;

namespace ApiSalonyar.Models
{
    public partial class Profession
    {
        public Profession()
        {
            staff = new HashSet<staff>();
        }

        public int ProfessionId { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<staff> staff { get; set; }
    }
}
