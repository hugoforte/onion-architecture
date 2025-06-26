using Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payments.Persistence.Configurations
{
    internal sealed class UserConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTable(nameof(User));

            builder.HasKey(user => user.Id);

            builder.Property(user => user.Id).ValueGeneratedOnAdd();

            builder.Property(user => user.PublicId).IsRequired();

            builder.Property(user => user.Email).HasMaxLength(255).IsRequired();

            builder.Property(user => user.Name).HasMaxLength(100).IsRequired();

            // Index on PublicId for faster lookups
            builder.HasIndex(user => user.PublicId).IsUnique();

            // Index on Email for login lookups
            builder.HasIndex(user => user.Email).IsUnique();

            // Foreign key relationships
            builder.HasOne(user => user.Customer)
                .WithMany(customer => customer.Users)
                .HasForeignKey(user => user.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 