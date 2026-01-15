using Starter.Services.Abstractions;

namespace Starter.Services;

internal sealed class ServiceManager : IServiceManager
{
    public ServiceManager(ITodoListService todoListService, ITodoItemService todoItemService, INotificationService notificationService)
    {
        TodoLists = todoListService;
        TodoItems = todoItemService;
        Notifications = notificationService;
    }

    public ITodoListService TodoLists { get; }
    public ITodoItemService TodoItems { get; }
    public INotificationService Notifications { get; }
}
