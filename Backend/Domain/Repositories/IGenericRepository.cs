using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Payments.Domain.Entities;

namespace Payments.Domain.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        
        Task<T> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default);
        
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        
        void Insert(T entity);
        
        void Update(T entity);
        
        void Remove(T entity);
        
        void RemoveRange(IEnumerable<T> entities);
    }
} 