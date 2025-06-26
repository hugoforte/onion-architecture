using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Payments.Contracts;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Mapster;
using Payments.Services.Abstractions;
using Payments.Domain.Exceptions;

namespace Payments.Services
{
    internal sealed class CustomerAddressService : ICustomerAddressService
    {
        private readonly IRepositoryManager _repositoryManager;

        public CustomerAddressService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<IEnumerable<CustomerAddressDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var addresses = await _repositoryManager.GetRepository<CustomerAddress>().GetAllAsync(cancellationToken);

            var addressesDto = addresses.Adapt<IEnumerable<CustomerAddressDto>>();

            return addressesDto;
        }

        public async Task<CustomerAddressDto> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var address = await _repositoryManager.GetRepository<CustomerAddress>().GetByIdAsync(id, cancellationToken);

            if (address is null)
            {
                throw new CustomerAddressNotFoundException(id);
            }

            var addressDto = address.Adapt<CustomerAddressDto>();

            return addressDto;
        }

        public async Task<CustomerAddressDto> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default)
        {
            var addresses = await _repositoryManager.GetRepository<CustomerAddress>().GetAllAsync(cancellationToken);
            var address = addresses.FirstOrDefault(a => a.PublicId == publicId);

            if (address is null)
            {
                throw new CustomerAddressNotFoundException(publicId);
            }

            var addressDto = address.Adapt<CustomerAddressDto>();

            return addressDto;
        }

        public async Task<IEnumerable<CustomerAddressDto>> GetByCustomerIdAsync(long customerId, CancellationToken cancellationToken = default)
        {
            var addresses = await _repositoryManager.GetRepository<CustomerAddress>().GetAllAsync(cancellationToken);
            var customerAddresses = addresses.Where(a => a.CustomerId == customerId);

            var addressesDto = customerAddresses.Adapt<IEnumerable<CustomerAddressDto>>();

            return addressesDto;
        }

        public async Task<CustomerAddressDto> CreateAsync(CustomerAddressForCreationDto customerAddressForCreationDto, CancellationToken cancellationToken = default)
        {
            var address = customerAddressForCreationDto.Adapt<CustomerAddress>();

            address.PublicId = Guid.NewGuid();
            address.CreatedAt = DateTime.UtcNow;
            address.UpdatedAt = DateTime.UtcNow;

            // If this is the first address for the customer, make it default
            if (address.IsDefault)
            {
                await ClearDefaultAddressForCustomer(address.CustomerId, cancellationToken);
            }

            _repositoryManager.GetRepository<CustomerAddress>().Insert(address);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return address.Adapt<CustomerAddressDto>();
        }

        public async Task UpdateAsync(long id, CustomerAddressForUpdateDto customerAddressForUpdateDto, CancellationToken cancellationToken = default)
        {
            var address = await _repositoryManager.GetRepository<CustomerAddress>().GetByIdAsync(id, cancellationToken);

            if (address is null)
            {
                throw new CustomerAddressNotFoundException(id);
            }

            // Handle default address logic
            if (customerAddressForUpdateDto.IsDefault == true && !address.IsDefault)
            {
                await ClearDefaultAddressForCustomer(address.CustomerId, cancellationToken);
            }

            customerAddressForUpdateDto.Adapt(address);
            address.UpdatedAt = DateTime.UtcNow;

            _repositoryManager.GetRepository<CustomerAddress>().Update(address);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            var address = await _repositoryManager.GetRepository<CustomerAddress>().GetByIdAsync(id, cancellationToken);

            if (address is null)
            {
                throw new CustomerAddressNotFoundException(id);
            }

            _repositoryManager.GetRepository<CustomerAddress>().Remove(address);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task ClearDefaultAddressForCustomer(long customerId, CancellationToken cancellationToken)
        {
            var addresses = await _repositoryManager.GetRepository<CustomerAddress>().GetAllAsync(cancellationToken);
            var defaultAddresses = addresses.Where(a => a.CustomerId == customerId && a.IsDefault);

            foreach (var defaultAddress in defaultAddresses)
            {
                defaultAddress.IsDefault = false;
                defaultAddress.UpdatedAt = DateTime.UtcNow;
                _repositoryManager.GetRepository<CustomerAddress>().Update(defaultAddress);
            }
        }
    }
} 