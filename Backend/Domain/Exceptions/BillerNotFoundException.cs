using System;

namespace Payments.Domain.Exceptions
{
    public sealed class BillerNotFoundException : NotFoundException
    {
        public BillerNotFoundException(long id)
            : base($"The biller with the identifier {id} was not found.")    
        {
        }

        public BillerNotFoundException(Guid publicId)
            : base($"The biller with the public identifier {publicId} was not found.")    
        {
        }
    }
} 