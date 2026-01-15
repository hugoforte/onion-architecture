using Starter.Domain.Entities;

namespace Starter.Domain.Repositories;

public interface ITodoItemRepository : IGenericRepository<TodoItem>
{
    Task<IReadOnlyCollection<TodoItem>> GetByListIdAsync(Guid listId, CancellationToken cancellationToken = default);
}
