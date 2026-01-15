using Mapster;
using Microsoft.Extensions.Logging;
using Starter.Contracts;
using Starter.Domain.Entities;
using Starter.Domain.Exceptions;
using Starter.Domain.Repositories;
using Starter.Services.Abstractions;

namespace Starter.Services;

internal sealed class TodoListService : ITodoListService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILogger<TodoListService> _logger;

    public TodoListService(IRepositoryManager repositoryManager, ILogger<TodoListService> logger)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<TodoListDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var lists = await _repositoryManager.TodoLists.GetAllWithItemsAsync(cancellationToken);
        return lists.Adapt<IReadOnlyCollection<TodoListDto>>();
    }

    public async Task<TodoListDto> GetByIdAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        var list = await _repositoryManager.TodoLists.GetWithItemsAsync(listId, cancellationToken)
                   ?? throw new TodoListNotFoundException(listId);

        return list.Adapt<TodoListDto>();
    }

    public async Task<TodoListDto> CreateAsync(TodoListForCreationDto dto, CancellationToken cancellationToken = default)
    {
        var list = dto.Adapt<TodoList>();
        _repositoryManager.TodoLists.Insert(list);
        await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created todo list {ListId}", list.Id);
        return list.Adapt<TodoListDto>();
    }

    public async Task UpdateAsync(Guid listId, TodoListForUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var list = await _repositoryManager.TodoLists.GetByIdAsync(listId, cancellationToken)
                   ?? throw new TodoListNotFoundException(listId);

        list.Name = dto.Name;
        list.Description = dto.Description;
        list.MarkUpdated();

        _repositoryManager.TodoLists.Update(list);
        await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated todo list {ListId}", list.Id);
    }

    public async Task DeleteAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        var list = await _repositoryManager.TodoLists.GetByIdAsync(listId, cancellationToken)
                   ?? throw new TodoListNotFoundException(listId);

        _repositoryManager.TodoLists.Remove(list);
        await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted todo list {ListId}", list.Id);
    }
}
