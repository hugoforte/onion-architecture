namespace Starter.Services.Abstractions;

public interface IServiceManager
{
    ITodoListService TodoLists { get; }
    ITodoItemService TodoItems { get; }
    INotificationService Notifications { get; }
}
