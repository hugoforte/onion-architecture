using Microsoft.EntityFrameworkCore;
using Starter.Domain.Entities;
using Starter.Domain.Repositories;

namespace Starter.Infrastructure.Database.Repositories;

internal sealed class TodoListRepository : RepositoryBase<TodoList>, ITodoListRepository
{
    public TodoListRepository(RepositoryDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyCollection<TodoList>> GetAllWithItemsAsync(CancellationToken cancellationToken = default)
    {
        var lists = await DbSet
            .Include(list => list.Items)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return lists;
    }

    public async Task<TodoList?> GetWithItemsAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(list => list.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(list => list.Id == listId, cancellationToken);
    }
}
