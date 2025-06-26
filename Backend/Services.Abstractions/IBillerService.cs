using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Payments.Contracts;

namespace Payments.Services.Abstractions
{
    public interface IBillerService
    {
        Task<IEnumerable<BillerDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<BillerDto> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        Task<BillerDto> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);

        Task<BillerDto> CreateAsync(BillerForCreationDto billerForCreationDto, CancellationToken cancellationToken = default);

        Task UpdateAsync(long id, BillerForUpdateDto billerForUpdateDto, CancellationToken cancellationToken = default);

        Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    }
} 