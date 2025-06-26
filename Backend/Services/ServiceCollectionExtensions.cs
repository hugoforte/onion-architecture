using Microsoft.Extensions.DependencyInjection;
using Payments.Services.Abstractions;
using Scrutor;

namespace Payments.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Register internal services explicitly since they are internal and Scrutor can't discover them
            services.AddScoped<IBillerService, BillerService>();
            services.AddScoped<ICustomerAddressService, CustomerAddressService>();

            // Register managers
            services.AddScoped<IServiceManager, ServiceManager>();

            return services;
        }
    }
} 