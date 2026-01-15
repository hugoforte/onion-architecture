using Microsoft.EntityFrameworkCore;
using Starter.Domain.Entities;
using Starter.Domain.Repositories;

namespace Starter.Infrastructure.Database.Repositories;

internal sealed class TodoItemRepository : RepositoryBase<TodoItem>, ITodoItemRepository
{
    public TodoItemRepository(RepositoryDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyCollection<TodoItem>> GetByListIdAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        var items = await DbSet.AsNoTracking()
            .Where(item => item.TodoListId == listId)
            .ToListAsync(cancellationToken);
        return items;
    }
}
