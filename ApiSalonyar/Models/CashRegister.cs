using System;
using System.Collections.Generic;
namespace ApiSalonyar.Models
{
    public partial class CashRegister
    {
        public CashRegister()
        {
            Payments = new HashSet<Payment>();
            Expenses = new HashSet<Expense>();
        }
        public int CashRegisterId { get; set; }
        public int BranchId { get; set; }
        public string Title { get; set; } = null!;
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Branch? Branch { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
        public virtual ICollection<Expense>? Expenses { get; set; }
    }
}
