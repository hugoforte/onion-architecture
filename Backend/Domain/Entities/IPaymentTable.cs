using System;

namespace Payments.Domain.Entities
{
    public interface IPaymentTable
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
} 