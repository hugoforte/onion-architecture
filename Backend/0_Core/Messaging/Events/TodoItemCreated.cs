using NServiceBus;
using Starter.Domain.Entities;

namespace Starter.Messaging.Events;

public class TodoItemCreated : IEvent
{
    public Guid TodoItemId { get; set; }
    public Guid TodoListId { get; set; }
    public string Title { get; set; } = string.Empty;
    public TodoPriority Priority { get; set; }
}
