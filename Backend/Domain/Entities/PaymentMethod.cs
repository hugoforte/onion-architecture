using System;
using System.Collections.Generic;

namespace Payments.Domain.Entities
{
    public class PaymentMethod : BaseEntity
    {
        public long Id { get; set; }
        
        public Guid PublicId { get; set; }
        
        public long CustomerId { get; set; }
        
        public long? OwnerUserId { get; set; }
        
        public string Token { get; set; }
        
        public string Type { get; set; }
        
        public string Last4 { get; set; }
        
        public string Brand { get; set; }
        
        public DateTime ExpiryDate { get; set; }
        
        public bool IsShared { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public User OwnerUser { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
} 