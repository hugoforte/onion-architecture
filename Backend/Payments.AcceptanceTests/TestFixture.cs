using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Payments.Persistence;
using Payments.Services.Abstractions;
using Payments.Services;
using Payments.Domain.Repositories;
using Payments.Persistence.Repositories;
using NUnit.Framework;

namespace Payments.AcceptanceTests
{
    [TestFixture]
    public class TestFixture : IAsyncDisposable
    {
        private ServiceProvider _serviceProvider;
        private RepositoryDbContext _dbContext;
        private string _dbName;

        public IBillerService BillerService { get; private set; }

        public TestFixture()
        {
            _dbName = $"TestDb_{Guid.NewGuid()}";
            var services = new ServiceCollection();

            // Add InMemory EF Core (override persistence)
            services.AddDbContext<RepositoryDbContext>(options => options.UseInMemoryDatabase(_dbName));

            // Register services using the same pattern as web project
            services.AddServices();

            // Register repositories using the same pattern as web project
            services.AddPersistenceRepositories();

            _serviceProvider = services.BuildServiceProvider();

            // Get the DbContext
            _dbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _dbContext.Database.EnsureCreated();

            // Get the BillerService
            BillerService = _serviceProvider.GetRequiredService<IBillerService>();
        }

        public async ValueTask DisposeAsync()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.DisposeAsync();
            if (_serviceProvider is IDisposable d)
                d.Dispose();
        }
    }
} 