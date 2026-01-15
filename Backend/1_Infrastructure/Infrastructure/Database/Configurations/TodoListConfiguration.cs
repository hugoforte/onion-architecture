using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starter.Domain.Entities;

namespace Starter.Infrastructure.Database.Configurations;

internal sealed class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.ToTable("TodoLists");
        builder.HasKey(list => list.Id);
        builder.Property(list => list.Name).IsRequired().HasMaxLength(150);
        builder.Property(list => list.Description).HasMaxLength(500);

        builder.HasMany(list => list.Items)
            .WithOne(item => item.TodoList!)
            .HasForeignKey(item => item.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
