using System;

namespace Payments.Contracts
{
    public class BillerDto
    {
        public long Id { get; set; }

        public Guid PublicId { get; set; }

        public string Name { get; set; }

        public string ApiKey { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
} 