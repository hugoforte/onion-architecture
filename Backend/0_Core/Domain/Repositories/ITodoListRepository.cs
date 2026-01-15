using Starter.Domain.Entities;

namespace Starter.Domain.Repositories;

public interface ITodoListRepository : IGenericRepository<TodoList>
{
    Task<IReadOnlyCollection<TodoList>> GetAllWithItemsAsync(CancellationToken cancellationToken = default);
    Task<TodoList?> GetWithItemsAsync(Guid listId, CancellationToken cancellationToken = default);
}
