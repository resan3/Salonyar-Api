using System;
using System.Collections.Generic;
namespace ApiSalonyar.Models
{
    public partial class ExpenseCategory
    {
        public ExpenseCategory()
        {
            Expenses = new HashSet<Expense>();
        }
        public int ExpenseCategoryId { get; set; }
        public string Title { get; set; } = null!;
        public bool IsDeleted { get; set; }

        public virtual ICollection<Expense>? Expenses { get; set; }
    }
}
