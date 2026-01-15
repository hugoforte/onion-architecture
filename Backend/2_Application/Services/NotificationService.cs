using Microsoft.Extensions.Logging;
using Starter.Domain.Entities;
using Starter.Services.Abstractions;

namespace Starter.Services;

internal sealed class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyCompletedAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Notified completion for todo item {ItemId}", item.Id);
        return Task.CompletedTask;
    }
}
