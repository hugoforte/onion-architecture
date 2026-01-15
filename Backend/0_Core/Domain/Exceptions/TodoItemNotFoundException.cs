namespace Starter.Domain.Exceptions;

public sealed class TodoItemNotFoundException : NotFoundException
{
    public TodoItemNotFoundException(Guid itemId) : base($"Todo item '{itemId}' was not found.")
    {
    }
}
