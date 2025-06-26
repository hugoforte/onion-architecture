using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Persistence.Repositories
{
    internal sealed class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly RepositoryDbContext _dbContext;

        public GenericRepository(RepositoryDbContext dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().Where(predicate).ToListAsync(cancellationToken);

        public async Task<T> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().AnyAsync(predicate, cancellationToken);

        public void Insert(T entity) => _dbContext.Set<T>().Add(entity);

        public void Update(T entity) => _dbContext.Set<T>().Update(entity);

        public void Remove(T entity) => _dbContext.Set<T>().Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => _dbContext.Set<T>().RemoveRange(entities);
    }
} 