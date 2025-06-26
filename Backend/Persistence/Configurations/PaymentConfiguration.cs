using Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payments.Persistence.Configurations
{
    internal sealed class PaymentConfiguration : BaseEntityConfiguration<Payment>
    {
        public override void Configure(EntityTypeBuilder<Payment> builder)
        {
            base.Configure(builder);

            builder.ToTable(nameof(Payment));

            builder.HasKey(payment => payment.Id);

            builder.Property(payment => payment.Id).ValueGeneratedOnAdd();

            builder.Property(payment => payment.PublicId).IsRequired();

            builder.Property(payment => payment.Status).HasMaxLength(50).IsRequired();

            builder.Property(payment => payment.AttemptCount).IsRequired();

            builder.Property(payment => payment.Amount).HasPrecision(18, 2).IsRequired();

            builder.Property(payment => payment.TransactionReference).HasMaxLength(255);

            // Index on PublicId for faster lookups
            builder.HasIndex(payment => payment.PublicId).IsUnique();

            // Foreign key relationships
            builder.HasOne(payment => payment.Invoice)
                .WithMany(invoice => invoice.Payments)
                .HasForeignKey(payment => payment.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(payment => payment.PaymentMethod)
                .WithMany(paymentMethod => paymentMethod.Payments)
                .HasForeignKey(payment => payment.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 