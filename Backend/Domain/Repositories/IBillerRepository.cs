using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Payments.Domain.Entities;

namespace Payments.Domain.Repositories
{
    public interface IBillerRepository
    {
        Task<IEnumerable<Biller>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Biller> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        Task<Biller> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);

        void Insert(Biller biller);

        void Update(Biller biller);

        void Remove(Biller biller);
    }
} 