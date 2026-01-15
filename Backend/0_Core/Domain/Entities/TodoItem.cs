namespace Starter.Domain.Entities;

public sealed class TodoItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; private set; }
    public DateTimeOffset? DueDate { get; set; }
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
    public Guid TodoListId { get; set; }
    public TodoList? TodoList { get; set; }

    public void MarkComplete()
    {
        IsCompleted = true;
        MarkUpdated();
    }
}
