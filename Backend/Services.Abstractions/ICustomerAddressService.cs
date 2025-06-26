using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Payments.Contracts;

namespace Payments.Services.Abstractions
{
    public interface ICustomerAddressService
    {
        Task<IEnumerable<CustomerAddressDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<CustomerAddressDto> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        Task<CustomerAddressDto> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);

        Task<IEnumerable<CustomerAddressDto>> GetByCustomerIdAsync(long customerId, CancellationToken cancellationToken = default);

        Task<CustomerAddressDto> CreateAsync(CustomerAddressForCreationDto customerAddressForCreationDto, CancellationToken cancellationToken = default);

        Task UpdateAsync(long id, CustomerAddressForUpdateDto customerAddressForUpdateDto, CancellationToken cancellationToken = default);

        Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    }
} 