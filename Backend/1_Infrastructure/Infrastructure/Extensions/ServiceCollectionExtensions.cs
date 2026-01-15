using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Starter.Common;
using Starter.Domain.Repositories;
using Starter.Infrastructure.Database;
using Starter.Infrastructure.Database.Repositories;

namespace Starter.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    private const string DefaultConnectionString = "Host=localhost;Port=5432;Database=starter;Username=postgres;Password=postgres";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ?? EnvironmentVariables.Get("DB_CONNECTION_STRING", DefaultConnectionString);

        services.AddDbContext<RepositoryDbContext>(options =>
        {
            if (connectionString.Contains("DataSource=", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseNpgsql(connectionString);
            }
        });

        services.AddScoped<ITodoListRepository, TodoListRepository>();
        services.AddScoped<ITodoItemRepository, TodoItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRepositoryManager, RepositoryManager>();

        return services;
    }
}
