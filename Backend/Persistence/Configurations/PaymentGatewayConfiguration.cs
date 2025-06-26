using Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payments.Persistence.Configurations
{
    internal sealed class PaymentGatewayConfiguration : BaseEntityConfiguration<PaymentGateway>
    {
        public override void Configure(EntityTypeBuilder<PaymentGateway> builder)
        {
            base.Configure(builder);

            builder.ToTable(nameof(PaymentGateway));

            builder.HasKey(gateway => gateway.Id);

            builder.Property(gateway => gateway.Id).ValueGeneratedOnAdd();

            builder.Property(gateway => gateway.Name).HasMaxLength(100).IsRequired();

            builder.Property(gateway => gateway.Type).HasMaxLength(50).IsRequired();

            // Index on Name for faster lookups
            builder.HasIndex(gateway => gateway.Name).IsUnique();
        }
    }
} 