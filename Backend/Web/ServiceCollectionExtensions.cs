using Microsoft.Extensions.DependencyInjection;
using Payments.Persistence;
using Payments.Services;

namespace Payments.Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString)
        {
            // Register persistence layer dependencies
            services.AddPersistence(connectionString);

            // Register service layer dependencies
            services.AddServices();

            return services;
        }
    }
} 