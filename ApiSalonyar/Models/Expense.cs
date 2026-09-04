using System;
namespace ApiSalonyar.Models
{
    public partial class Expense
    {
        public int ExpenseId { get; set; }
        public int BranchId { get; set; }
        public int? CashRegisterId { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public int? PaymentMethodId { get; set; }
        public string? Description { get; set; }
        public string? ReceiptImagePath { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Branch?           Branch          { get; set; }
        public virtual CashRegister?     CashRegister    { get; set; }
        public virtual ExpenseCategory?  Category        { get; set; }
        public virtual PaymentMethod?    PaymentMethod   { get; set; }
        public virtual User?             CreatedByUser   { get; set; }
    }
}
