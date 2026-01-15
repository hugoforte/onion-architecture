using NServiceBus;
using Starter.Domain.Entities;

namespace Starter.Messaging.Commands;

public class CreateTodoItem : ICommand
{
    public Guid TodoListId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
}
