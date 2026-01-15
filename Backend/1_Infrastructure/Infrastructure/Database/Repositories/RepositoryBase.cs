using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Starter.Domain.Entities;
using Starter.Domain.Repositories;

namespace Starter.Infrastructure.Database.Repositories;

internal class RepositoryBase<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly RepositoryDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected RepositoryBase(RepositoryDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await DbSet.AsNoTracking().ToListAsync(cancellationToken);
        return entities;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        return entities;
    }

    public void Insert(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}
