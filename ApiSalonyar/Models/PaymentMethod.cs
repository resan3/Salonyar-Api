using System;
using System.Collections.Generic;
namespace ApiSalonyar.Models
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            Payments = new HashSet<Payment>();
            Expenses = new HashSet<Expense>();
        }
        public int PaymentMethodId { get; set; }
        public string Title { get; set; } = null!;
        public string Code { get; set; } = null!;
        public bool RequiresCode { get; set; }
        public string? CodeLabel { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Payment>? Payments { get; set; }
        public virtual ICollection<Expense>? Expenses { get; set; }
    }
}
