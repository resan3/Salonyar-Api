using System;
namespace ApiSalonyar.Models
{
    public partial class Refund
    {
        public int RefundId { get; set; }
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = null!;
        public DateTime RefundedAt { get; set; }
        public int? RefundedByUserId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Payment? Payment          { get; set; }
        public virtual Invoice? Invoice          { get; set; }
        public virtual User?    RefundedByUser   { get; set; }
    }
}
