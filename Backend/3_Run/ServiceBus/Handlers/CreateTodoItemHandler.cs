using NServiceBus;
using Starter.Contracts;
using Starter.Messaging.Commands;
using Starter.Messaging.Events;
using Starter.Services.Abstractions;

namespace Starter.ServiceBus.Handlers;

public sealed class CreateTodoItemHandler : IHandleMessages<CreateTodoItem>
{
    private readonly ITodoItemService _todoItemService;

    public CreateTodoItemHandler(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }

    public async Task Handle(CreateTodoItem message, IMessageHandlerContext context)
    {
        var dto = new TodoItemForCreationDto
        {
            Title = message.Title,
            Description = message.Description,
            DueDate = message.DueDate,
            Priority = message.Priority,
            TodoListId = message.TodoListId
        };

        var created = await _todoItemService.CreateAsync(dto, context.CancellationToken);

        var createdEvent = new TodoItemCreated
        {
            TodoItemId = created.Id,
            TodoListId = created.TodoListId,
            Title = created.Title,
            Priority = created.Priority
        };

        await context.Publish(createdEvent).ConfigureAwait(false);
    }
}
