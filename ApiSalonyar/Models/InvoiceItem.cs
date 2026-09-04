using System;
namespace ApiSalonyar.Models
{
    public partial class InvoiceItem
    {
        public int InvoiceItemId { get; set; }
        public int InvoiceId { get; set; }
        public int? TreatmentId { get; set; }
        public string Description { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }

        public virtual Invoice?   Invoice   { get; set; }
        public virtual Treatment? Treatment { get; set; }
    }
}
