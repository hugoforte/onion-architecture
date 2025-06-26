using System;
using System.Collections.Generic;
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
    internal sealed class BillerService : IBillerService
    {
        private readonly IRepositoryManager _repositoryManager;

        public BillerService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<IEnumerable<BillerDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            // Example using generic repo
            var billers = await _repositoryManager.GetRepository<Biller>().GetAllAsync(cancellationToken);

            var billersDto = billers.Adapt<IEnumerable<BillerDto>>();

            return billersDto;
        }

        public async Task<BillerDto> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var biller = await _repositoryManager.BillerRepository.GetByIdAsync(id, cancellationToken);

            if (biller is null)
            {
                throw new BillerNotFoundException(id);
            }

            var billerDto = biller.Adapt<BillerDto>();

            return billerDto;
        }

        public async Task<BillerDto> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default)
        {
            var biller = await _repositoryManager.BillerRepository.GetByPublicIdAsync(publicId, cancellationToken);

            if (biller is null)
            {
                throw new BillerNotFoundException(publicId);
            }

            var billerDto = biller.Adapt<BillerDto>();

            return billerDto;
        }

        public async Task<BillerDto> CreateAsync(BillerForCreationDto billerForCreationDto, CancellationToken cancellationToken = default)
        {
            var biller = billerForCreationDto.Adapt<Biller>();

            biller.PublicId = Guid.NewGuid();
            biller.CreatedAt = DateTime.UtcNow;
            biller.UpdatedAt = DateTime.UtcNow;

            _repositoryManager.BillerRepository.Insert(biller);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return biller.Adapt<BillerDto>();
        }

        public async Task UpdateAsync(long id, BillerForUpdateDto billerForUpdateDto, CancellationToken cancellationToken = default)
        {
            var biller = await _repositoryManager.BillerRepository.GetByIdAsync(id, cancellationToken);

            if (biller is null)
            {
                throw new BillerNotFoundException(id);
            }

            billerForUpdateDto.Adapt(biller);
            biller.UpdatedAt = DateTime.UtcNow;

            _repositoryManager.BillerRepository.Update(biller);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            var biller = await _repositoryManager.BillerRepository.GetByIdAsync(id, cancellationToken);

            if (biller is null)
            {
                throw new BillerNotFoundException(id);
            }

            _repositoryManager.BillerRepository.Remove(biller);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
} 