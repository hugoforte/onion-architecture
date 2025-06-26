using Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payments.Persistence.Configurations
{
    internal sealed class BillerConfiguration : BaseEntityConfiguration<Biller>
    {
        public override void Configure(EntityTypeBuilder<Biller> builder)
        {
            base.Configure(builder);

            builder.ToTable(nameof(Biller));

            builder.HasKey(biller => biller.Id);

            builder.Property(biller => biller.Id).ValueGeneratedOnAdd();

            builder.Property(biller => biller.PublicId).IsRequired();

            builder.Property(biller => biller.Name).HasMaxLength(255).IsRequired();

            builder.Property(biller => biller.ApiKey).HasMaxLength(255).IsRequired();

            // Index on PublicId for faster lookups
            builder.HasIndex(biller => biller.PublicId).IsUnique();
        }
    }
} 