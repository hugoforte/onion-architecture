using System.Linq.Expressions;
using Starter.Domain.Entities;

namespace Starter.Domain.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    void Insert(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
