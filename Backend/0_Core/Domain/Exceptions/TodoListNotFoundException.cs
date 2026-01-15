namespace Starter.Domain.Exceptions;

public sealed class TodoListNotFoundException : NotFoundException
{
    public TodoListNotFoundException(Guid listId) : base($"Todo list '{listId}' was not found.")
    {
    }
}
