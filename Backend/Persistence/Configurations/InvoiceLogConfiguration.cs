using Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Payments.Persistence.Configurations
{
    internal sealed class InvoiceLogConfiguration : BaseEntityConfiguration<InvoiceLog>
    {
        public override void Configure(EntityTypeBuilder<InvoiceLog> builder)
        {
            base.Configure(builder);

            var jsonConverter = new ValueConverter<JsonDocument, string>(
                v => v == null ? null : v.RootElement.ToString(),
                v => v == null ? null : JsonDocument.Parse(v, default)
            );

            builder.ToTable(nameof(InvoiceLog));

            builder.HasKey(log => log.Id);

            builder.Property(log => log.Id).ValueGeneratedOnAdd();

            builder.Property(log => log.Object).HasMaxLength(100).IsRequired();

            builder.Property(log => log.ObjectId).IsRequired();

            builder.Property(log => log.Changes)
                .HasColumnType("jsonb")
                .HasConversion(jsonConverter);

            // Index on Object and ObjectId for faster lookups
            builder.HasIndex(log => new { log.Object, log.ObjectId });
        }
    }
} 