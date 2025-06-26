using Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payments.Persistence.Configurations
{
    internal sealed class CustomerConfiguration : BaseEntityConfiguration<Customer>
    {
        public override void Configure(EntityTypeBuilder<Customer> builder)
        {
            base.Configure(builder);

            builder.ToTable(nameof(Customer));

            builder.HasKey(customer => customer.Id);

            builder.Property(customer => customer.Id).ValueGeneratedOnAdd();

            builder.Property(customer => customer.PublicId).IsRequired();

            builder.Property(customer => customer.Name).HasMaxLength(100).IsRequired();

            builder.Property(customer => customer.AutopayEnabled).IsRequired();

            // Index on PublicId for faster lookups
            builder.HasIndex(customer => customer.PublicId).IsUnique();

            // Foreign key relationships
            builder.HasOne(customer => customer.Biller)
                .WithMany(biller => biller.Customers)
                .HasForeignKey(customer => customer.BillerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(customer => customer.PaymentGateway)
                .WithMany(gateway => gateway.Customers)
                .HasForeignKey(customer => customer.PaymentGatewayId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 