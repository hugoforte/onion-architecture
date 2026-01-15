using System.Collections.Generic;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Starter.Contracts;
using Starter.Infrastructure.Database;

namespace Starter.AcceptanceTests;

public sealed class TestFixture : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public TestFixture()
    {
        Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", _connection.ConnectionString);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Database", _connection.ConnectionString);

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = _connection.ConnectionString
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<RepositoryDbContext>));
            services.RemoveAll<RepositoryDbContext>();

            services.AddDbContext<RepositoryDbContext>(options => options.UseSqlite(_connection));
        });
    }

    public async Task InitializeAsync()
    {
        _connection.Open();
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public new Task DisposeAsync()
    {
        _connection.Dispose();
        return Task.CompletedTask;
    }

    public async Task<TodoListDto> CreateListAsync(HttpClient client, string name = "My List")
    {
        var response = await client.PostAsJsonAsync("/api/todolists", new TodoListForCreationDto { Name = name });
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TodoListDto>();
        return created!;
    }
}
