using Starter.Contracts;

namespace Starter.Services.Abstractions;

public interface ITodoItemService
{
    Task<IReadOnlyCollection<TodoItemDto>> GetByListAsync(Guid listId, CancellationToken cancellationToken = default);
    Task<TodoItemDto> GetByIdAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<TodoItemDto> CreateAsync(TodoItemForCreationDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid itemId, TodoItemForUpdateDto dto, CancellationToken cancellationToken = default);
    Task CompleteAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid itemId, CancellationToken cancellationToken = default);
}
