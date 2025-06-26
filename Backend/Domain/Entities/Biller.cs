using System;
using System.Collections.Generic;

namespace Payments.Domain.Entities
{
    public class Biller : BaseEntity
    {
        public long Id { get; set; }
        
        public Guid PublicId { get; set; }
        
        public string Name { get; set; }
        
        public string ApiKey { get; set; }

        // Navigation properties
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<BillerConfiguration> BillerConfigurations { get; set; }
    }
} 