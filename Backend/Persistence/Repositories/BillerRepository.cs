using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Payments.Persistence.Repositories
{
    internal sealed class BillerRepository : IBillerRepository
    {
        private readonly RepositoryDbContext _dbContext;

        public BillerRepository(RepositoryDbContext dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<Biller>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.Billers.ToListAsync(cancellationToken);

        public async Task<Biller> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
            await _dbContext.Billers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<Biller> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default) =>
            await _dbContext.Billers.FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);

        public void Insert(Biller biller) => _dbContext.Billers.Add(biller);

        public void Update(Biller biller) => _dbContext.Billers.Update(biller);

        public void Remove(Biller biller) => _dbContext.Billers.Remove(biller);
    }
} 