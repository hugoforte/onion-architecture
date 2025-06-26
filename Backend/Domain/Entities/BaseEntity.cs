using System;

namespace Payments.Domain.Entities
{
    public abstract class BaseEntity : IPaymentTable
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Audit methods - these will be called automatically by EF Core
        public void SetCreatedAt(DateTime createdAt) => CreatedAt = createdAt;
        public void SetUpdatedAt(DateTime updatedAt) => UpdatedAt = updatedAt;
    }
} 