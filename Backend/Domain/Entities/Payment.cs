using System;

namespace Payments.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public long Id { get; set; }
        
        public Guid PublicId { get; set; }
        
        public long InvoiceId { get; set; }
        
        public long PaymentMethodId { get; set; }
        
        public string Status { get; set; }
        
        public int AttemptCount { get; set; }
        
        public decimal Amount { get; set; }
        
        public string TransactionReference { get; set; }

        // Navigation properties
        public Invoice Invoice { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
} 