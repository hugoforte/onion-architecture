using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starter.Domain.Entities;

namespace Starter.Infrastructure.Database.Configurations;

internal sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("TodoItems");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(item => item.Description)
            .HasMaxLength(2000);

        builder.Property(item => item.Priority)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(item => item.IsCompleted)
            .IsRequired();

        builder.Property(item => item.DueDate);

        builder.HasIndex(item => item.TodoListId);
    }
}
