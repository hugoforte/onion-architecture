using Starter.Domain.Repositories;

namespace Starter.Infrastructure.Database;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly RepositoryDbContext _context;

    public UnitOfWork(RepositoryDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
