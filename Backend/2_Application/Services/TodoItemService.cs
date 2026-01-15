using Mapster;
using Microsoft.Extensions.Logging;
using Starter.Contracts;
using Starter.Domain.Entities;
using Starter.Domain.Exceptions;
using Starter.Domain.Repositories;
using Starter.Services.Abstractions;

namespace Starter.Services;

internal sealed class TodoItemService : ITodoItemService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly INotificationService _notificationService;
    private readonly ILogger<TodoItemService> _logger;

    public TodoItemService(IRepositoryManager repositoryManager, INotificationService notificationService, ILogger<TodoItemService> logger)
    {
        _repositoryManager = repositoryManager;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<TodoItemDto>> GetByListAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        var items = await _repositoryManager.TodoItems.GetByListIdAsync(listId, cancellationToken);
        return items.Adapt<IReadOnlyCollection<TodoItemDto>>();
    }

    public async Task<TodoItemDto> GetByIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = await _repositoryManager.TodoItems.GetByIdAsync(itemId, cancellationToken)
                   ?? throw new TodoItemNotFoundException(itemId);

        return item.Adapt<TodoItemDto>();
    }

    public async Task<TodoItemDto> CreateAsync(TodoItemForCreationDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureListExistsAsync(dto.TodoListId, cancellationToken);

        var item = dto.Adapt<TodoItem>();
        _repositoryManager.TodoItems.Insert(item);
        await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created todo item {ItemId} in list {ListId}", item.Id, item.TodoListId);
        return item.Adapt<TodoItemDto>();
    }

    public async Task UpdateAsync(Guid itemId, TodoItemForUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _repositoryManager.TodoItems.GetByIdAsync(itemId, cancellationToken)
                   ?? throw new TodoItemNotFoundException(itemId);

        item.Title = dto.Title;
        item.Description = dto.Description;
        item.DueDate = dto.DueDate;
        item.Priority = dto.Priority;
        item.MarkUpdated();

        _repositoryManager.TodoItems.Update(item);
        await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated todo item {ItemId}", item.Id);
    }

    public async Task CompleteAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = await _repositoryManager.TodoItems.GetByIdAsync(itemId, cancellationToken)
                   ?? throw new TodoItemNotFoundException(itemId);

        if (!item.IsCompleted)
        {
            item.MarkComplete();
            _repositoryManager.TodoItems.Update(item);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            await _notificationService.NotifyCompletedAsync(item, cancellationToken);
            _logger.LogInformation("Completed todo item {ItemId}", item.Id);
        }
    }

    public async Task DeleteAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = await _repositoryManager.TodoItems.GetByIdAsync(itemId, cancellationToken)
                   ?? throw new TodoItemNotFoundException(itemId);

        _repositoryManager.TodoItems.Remove(item);
        await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted todo item {ItemId}", item.Id);
    }

    private async Task EnsureListExistsAsync(Guid listId, CancellationToken cancellationToken)
    {
        var list = await _repositoryManager.TodoLists.GetByIdAsync(listId, cancellationToken);
        if (list is null)
        {
            throw new TodoListNotFoundException(listId);
        }
    }
}
