using System;
using System.Collections.Generic;
namespace ApiSalonyar.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            InvoiceItems      = new HashSet<InvoiceItem>();
            Payments          = new HashSet<Payment>();
            Refunds           = new HashSet<Refund>();
        //    CommissionRecords = new HashSet<CommissionRecord>();
            Notifications     = new HashSet<Notification>();
        }
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public int BranchId { get; set; }
        public int PatientId { get; set; }
        public int? VisitId { get; set; }
        public int? ReservationId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; } = "PENDING";
        public string? Notes { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Branch?          Branch          { get; set; }
        public virtual Patient?         Patient         { get; set; }
        public virtual PatientVisit?    Visit           { get; set; }
        public virtual RoomReservation? Reservation     { get; set; }
        public virtual User?            CreatedByUser   { get; set; }
        public virtual ICollection<InvoiceItem>?      InvoiceItems      { get; set; }
        public virtual ICollection<Payment>?          Payments          { get; set; }
        public virtual ICollection<Refund>?           Refunds           { get; set; }
     //   public virtual ICollection<CommissionRecord>? CommissionRecords { get; set; }
        public virtual ICollection<Notification>?     Notifications     { get; set; }
    }
}
