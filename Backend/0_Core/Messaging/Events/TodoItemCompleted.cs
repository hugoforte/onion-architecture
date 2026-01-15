using NServiceBus;

namespace Starter.Messaging.Events;

public class TodoItemCompleted : IEvent
{
    public Guid TodoItemId { get; set; }
    public Guid TodoListId { get; set; }
}
