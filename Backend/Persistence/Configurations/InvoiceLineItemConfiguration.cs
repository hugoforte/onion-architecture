using Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payments.Persistence.Configurations
{
    internal sealed class InvoiceLineItemConfiguration : BaseEntityConfiguration<InvoiceLineItem>
    {
        public override void Configure(EntityTypeBuilder<InvoiceLineItem> builder)
        {
            base.Configure(builder);

            builder.ToTable(nameof(InvoiceLineItem));

            builder.HasKey(lineItem => lineItem.Id);

            builder.Property(lineItem => lineItem.Id).ValueGeneratedOnAdd();

            builder.Property(lineItem => lineItem.Description).HasMaxLength(500).IsRequired();

            builder.Property(lineItem => lineItem.Quantity).IsRequired();

            builder.Property(lineItem => lineItem.UnitPrice).HasPrecision(18, 2).IsRequired();

            // Foreign key relationships
            builder.HasOne(lineItem => lineItem.Invoice)
                .WithMany(invoice => invoice.InvoiceLineItems)
                .HasForeignKey(lineItem => lineItem.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 