using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Payments.Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public long Id { get; set; }
        
        public Guid PublicId { get; set; }
        
        public long BillerId { get; set; }
        
        public long CustomerId { get; set; }
        
        public DateTime DueDate { get; set; }
        
        public JsonDocument Fees { get; set; }
        
        public bool PassThruFees { get; set; }
        
        public decimal SalesTax { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        public string Currency { get; set; }
        
        public string Status { get; set; }

        // Navigation properties
        public Biller Biller { get; set; }
        public Customer Customer { get; set; }
        public ICollection<InvoiceLineItem> InvoiceLineItems { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
} 