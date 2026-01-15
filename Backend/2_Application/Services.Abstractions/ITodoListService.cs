using Starter.Contracts;

namespace Starter.Services.Abstractions;

public interface ITodoListService
{
    Task<IReadOnlyCollection<TodoListDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TodoListDto> GetByIdAsync(Guid listId, CancellationToken cancellationToken = default);
    Task<TodoListDto> CreateAsync(TodoListForCreationDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid listId, TodoListForUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid listId, CancellationToken cancellationToken = default);
}
