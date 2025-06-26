using System;

namespace Payments.Domain.Entities
{
    public class InvoiceLineItem : BaseEntity
    {
        public long Id { get; set; }
        
        public long InvoiceId { get; set; }
        
        public string Description { get; set; }
        
        public int Quantity { get; set; }
        
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public Invoice Invoice { get; set; }
    }
} 