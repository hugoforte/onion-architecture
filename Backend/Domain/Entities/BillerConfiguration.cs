using System;

namespace Payments.Domain.Entities
{
    public class BillerConfiguration : BaseEntity
    {
        public long Id { get; set; }
        
        public long BillerId { get; set; }
        
        public string ConfigType { get; set; }
        
        public string ConfigValue { get; set; }

        // Navigation property
        public Biller Biller { get; set; }
    }
} 