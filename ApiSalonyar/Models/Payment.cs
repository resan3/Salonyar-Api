using System;
using System.Collections.Generic;
namespace ApiSalonyar.Models
{
    public partial class Payment
    {
        public Payment()
        {
            Refunds = new HashSet<Refund>();
        }
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public int? CashRegisterId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
        public string? ConfirmationCode { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime PaidAt { get; set; }
        public int? CollectedByStaffId { get; set; }
        public int? CollectedByUserId { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public int? VerifiedByUserId { get; set; }
        public string? Notes { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Invoice?       Invoice            { get; set; }
        public virtual CashRegister?  CashRegister       { get; set; }
        public virtual PaymentMethod? PaymentMethod      { get; set; }
        public virtual staff?         CollectedByStaff   { get; set; }
        public virtual User?          CollectedByUser    { get; set; }
        public virtual User?          VerifiedByUser     { get; set; }
        public virtual ICollection<Refund>? Refunds      { get; set; }
    }
}
