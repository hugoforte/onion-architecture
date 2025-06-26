using System;
using System.Text.Json;

namespace Payments.Domain.Entities
{
    public class InvoiceLog : BaseEntity
    {
        public long Id { get; set; }
        
        public string Object { get; set; }
        
        public int ObjectId { get; set; }
        
        public JsonDocument Changes { get; set; }
    }
} 