using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Repositories;
using Payments.Persistence.Repositories;
using Scrutor;
using System;

namespace Payments.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistenceDbContext(this IServiceCollection services, string connectionString)
        {
            // Only register PostgreSQL if not already using InMemory
            if (!connectionString.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
            {
                // Register database context
                services.AddDbContextPool<RepositoryDbContext>(builder =>
                {
                    builder.UseNpgsql(connectionString);
                });
            }

            return services;
        }

        public static IServiceCollection AddPersistenceRepositories(this IServiceCollection services)
        {
            // Register internal repositories explicitly since Scrutor can't scan them from other assemblies
            services.AddScoped<IBillerRepository, BillerRepository>();

            // Auto-register all other repositories that implement interfaces
            services.Scan(scan => scan
                .FromAssemblyOf<IBillerRepository>()
                .AddClasses(classes => classes.Where(type => 
                    type.Name.EndsWith("Repository") && 
                    !type.IsAbstract && 
                    !type.IsInterface &&
                    type != typeof(BillerRepository)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // Register open generic repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register repository manager
            services.AddScoped<IRepositoryManager, RepositoryManager>();

            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            services.AddPersistenceDbContext(connectionString);
            services.AddPersistenceRepositories();
            return services;
        }
    }
} 