using HotelManagementSystem.Domain.Entities.Billing;
using HotelManagementSystem.Domain.Entities.Billing.Payments;
using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.Entities.Hotels;
using HotelManagementSystem.Domain.Entities.Stays;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Persistence.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(i => i.IssuedAt);
        builder.Property(i => i.PaidAt);


        builder.ComplexProperty(i => i.SubTotal,
            m => m.ConfigureMoney("SubTotal", "SubTotalCurrency"));
        builder.ComplexProperty(i => i.DiscountAmount,
            m => m.ConfigureMoney("DiscountAmount", "DiscountCurrency"));
        builder.ComplexProperty(i => i.TaxAmount,
            m => m.ConfigureMoney("TaxAmount", "TaxCurrency"));
        builder.ComplexProperty(i => i.TotalAmount,
            m => m.ConfigureMoney("TotalAmount", "TotalCurrency"));


        builder.HasMany(i => i.LineItems)
            .WithOne()
            .HasForeignKey("InvoiceId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(i => i.LineItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);


        builder.HasOne(i => i.Payment)
            .WithOne()
            .HasForeignKey<Payment>(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Stay)
            .WithOne()
            .HasForeignKey<Invoice>(i => i.StayId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.StayId).IsUnique(true);
        builder.HasIndex(i => i.Status);

        builder.Ignore(i => i.DomainEvents);
    }
}