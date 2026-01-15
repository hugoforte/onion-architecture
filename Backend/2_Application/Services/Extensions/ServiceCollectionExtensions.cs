using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Starter.Services.Abstractions;

namespace Starter.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Mapster defaults are sufficient for matching DTO and entity property names
        TypeAdapterConfig.GlobalSettings.Default.Settings.PreserveReference = true;

        services.AddScoped<ITodoListService, TodoListService>();
        services.AddScoped<ITodoItemService, TodoItemService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IServiceManager, ServiceManager>();

        return services;
    }
}
