using System;
using System.Collections.Generic;

namespace Payments.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public long Id { get; set; }
        
        public Guid PublicId { get; set; }
        
        public long BillerId { get; set; }
        
        public long PaymentGatewayId { get; set; }
        
        public string Name { get; set; }
        
        public bool AutopayEnabled { get; set; }

        // Navigation properties
        public Biller Biller { get; set; }
        public PaymentGateway PaymentGateway { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<PaymentMethod> PaymentMethods { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<CustomerAddress> Addresses { get; set; }
    }
} 