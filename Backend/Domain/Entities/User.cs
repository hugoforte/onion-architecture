using System;
using System.Collections.Generic;

namespace Payments.Domain.Entities
{
    public class User : BaseEntity
    {
        public long Id { get; set; }
        
        public Guid PublicId { get; set; }
        
        public long CustomerId { get; set; }
        
        public string Email { get; set; }
        
        public string Name { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public ICollection<PaymentMethod> PaymentMethods { get; set; }
    }
} 