using Microsoft.EntityFrameworkCore;
using Starter.Infrastructure.Database;
using Starter.Infrastructure.Extensions;
using Starter.Services.Extensions;
using Starter.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevelopmentPolicy");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();

    if (dbContext.Database.IsSqlite())
    {
        dbContext.Database.EnsureCreated();
    }
    else
    {
        dbContext.Database.Migrate();
    }
}

app.Run();

public partial class Program;
