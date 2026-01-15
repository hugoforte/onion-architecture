using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Starter.Infrastructure.Database;

public sealed class RepositoryDbContextFactory : IDesignTimeDbContextFactory<RepositoryDbContext>
{
    public RepositoryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RepositoryDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=starter;Username=postgres;Password=postgres");
        return new RepositoryDbContext(optionsBuilder.Options);
    }
}
