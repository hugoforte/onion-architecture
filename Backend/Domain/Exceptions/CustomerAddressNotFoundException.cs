using System;

namespace Payments.Domain.Exceptions
{
    public sealed class CustomerAddressNotFoundException : NotFoundException
    {
        public CustomerAddressNotFoundException(long id)
            : base($"The customer address with id: {id} doesn't exist in the database.") { }

        public CustomerAddressNotFoundException(Guid publicId)
            : base($"The customer address with public id: {publicId} doesn't exist in the database.") { }
    }
} 