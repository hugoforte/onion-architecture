using Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payments.Persistence.Configurations
{
    internal sealed class PaymentMethodConfiguration : BaseEntityConfiguration<PaymentMethod>
    {
        public override void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            base.Configure(builder);

            builder.ToTable(nameof(PaymentMethod));

            builder.HasKey(paymentMethod => paymentMethod.Id);

            builder.Property(paymentMethod => paymentMethod.Id).ValueGeneratedOnAdd();

            builder.Property(paymentMethod => paymentMethod.PublicId).IsRequired();

            builder.Property(paymentMethod => paymentMethod.Token).HasMaxLength(255).IsRequired();

            builder.Property(paymentMethod => paymentMethod.Type).HasMaxLength(50).IsRequired();

            builder.Property(paymentMethod => paymentMethod.Last4).HasMaxLength(4).IsRequired();

            builder.Property(paymentMethod => paymentMethod.Brand).HasMaxLength(50).IsRequired();

            builder.Property(paymentMethod => paymentMethod.ExpiryDate).IsRequired();

            builder.Property(paymentMethod => paymentMethod.IsShared).IsRequired();

            // Index on PublicId for faster lookups
            builder.HasIndex(paymentMethod => paymentMethod.PublicId).IsUnique();

            // Foreign key relationships
            builder.HasOne(paymentMethod => paymentMethod.Customer)
                .WithMany(customer => customer.PaymentMethods)
                .HasForeignKey(paymentMethod => paymentMethod.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(paymentMethod => paymentMethod.OwnerUser)
                .WithMany(user => user.PaymentMethods)
                .HasForeignKey(paymentMethod => paymentMethod.OwnerUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
} 