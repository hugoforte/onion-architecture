using Starter.Domain.Entities;

namespace Starter.Services.Abstractions;

public interface INotificationService
{
    Task NotifyCompletedAsync(TodoItem item, CancellationToken cancellationToken = default);
}
