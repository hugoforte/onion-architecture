using System;
using System.Collections.Generic;

namespace Payments.Domain.Entities
{
    public class PaymentGateway : BaseEntity
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Type { get; set; }

        // Navigation properties
        public ICollection<Customer> Customers { get; set; }
    }
} 