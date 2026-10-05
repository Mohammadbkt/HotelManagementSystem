using HotelManagementSystem.Domain.Entities.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Persistence.Configurations;

public sealed class InvoiceLineItemConfiguration : IEntityTypeConfiguration<InvoiceLineItem>
{
    public void Configure(EntityTypeBuilder<InvoiceLineItem> builder)
    {
        builder.ToTable("InvoiceLineItems");

        builder.HasKey(li => li.Id);

        builder.Property(li => li.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(li => li.Quantity).IsRequired();
        builder.Property(li => li.LineNumber).IsRequired();

        builder.ComplexProperty(li => li.UnitPrice,
            m => m.ConfigureMoney("UnitPrice", "UnitPriceCurrency"));
        builder.ComplexProperty(li => li.LineTotal,
            m => m.ConfigureMoney("LineTotal", "LineTotalCurrency"));


        builder.HasIndex("InvoiceId", nameof(InvoiceLineItem.LineNumber))
            .IsUnique();

        builder.Ignore(li => li.DomainEvents);
    }
}