using Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Payments.Persistence.Configurations
{
    internal sealed class InvoiceConfiguration : BaseEntityConfiguration<Invoice>
    {
        private static JsonDocument ParseJson(string v) => v == null ? null : JsonDocument.Parse(v);

        public override void Configure(EntityTypeBuilder<Invoice> builder)
        {
            base.Configure(builder);

            var jsonConverter = new ValueConverter<JsonDocument, string>(
                v => v == null ? null : v.RootElement.ToString(),
                v => v == null ? null : JsonDocument.Parse(v, default)
            );

            builder.ToTable(nameof(Invoice));

            builder.HasKey(invoice => invoice.Id);

            builder.Property(invoice => invoice.Id).ValueGeneratedOnAdd();

            builder.Property(invoice => invoice.PublicId).IsRequired();

            builder.Property(invoice => invoice.DueDate).IsRequired();

            builder.Property(invoice => invoice.Fees)
                .HasColumnType("jsonb")
                .HasConversion(jsonConverter);

            builder.Property(invoice => invoice.PassThruFees).IsRequired();

            builder.Property(invoice => invoice.SalesTax).HasPrecision(18, 2).IsRequired();

            builder.Property(invoice => invoice.TotalAmount).HasPrecision(18, 2).IsRequired();

            builder.Property(invoice => invoice.Currency).HasMaxLength(3).IsRequired();

            builder.Property(invoice => invoice.Status).HasMaxLength(50).IsRequired();

            // Index on PublicId for faster lookups
            builder.HasIndex(invoice => invoice.PublicId).IsUnique();

            // Foreign key relationships
            builder.HasOne(invoice => invoice.Biller)
                .WithMany(biller => biller.Invoices)
                .HasForeignKey(invoice => invoice.BillerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(invoice => invoice.Customer)
                .WithMany(customer => customer.Invoices)
                .HasForeignKey(invoice => invoice.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 