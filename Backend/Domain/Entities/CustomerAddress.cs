using System;
using Payments.Domain.Enums;

namespace Payments.Domain.Entities
{
    public class CustomerAddress : BaseEntity
    {
        public long Id { get; set; }
        
        public Guid PublicId { get; set; }
        
        public long CustomerId { get; set; }
        
        public AddressType AddressType { get; set; }
        
        public string StreetAddress1 { get; set; }
        
        public string StreetAddress2 { get; set; }
        
        public string City { get; set; }
        
        public string State { get; set; }
        
        public string PostalCode { get; set; }
        
        public string Country { get; set; } = "US";
        
        public bool IsDefault { get; set; }

        // Navigation property
        public Customer Customer { get; set; }
    }
} 