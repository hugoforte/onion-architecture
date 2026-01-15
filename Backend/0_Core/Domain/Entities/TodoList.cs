namespace Starter.Domain.Entities;

public sealed class TodoList : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<TodoItem> Items { get; set; } = new List<TodoItem>();
}
