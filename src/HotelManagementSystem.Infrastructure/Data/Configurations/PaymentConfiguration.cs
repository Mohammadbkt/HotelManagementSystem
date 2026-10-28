using HotelManagementSystem.Domain.Entities.Billing.Payments;
using HotelManagementSystem.Domain.Entities.Guests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Method)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.PaymentDate).IsRequired();

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        builder.ComplexProperty(p => p.Amount,
            m => m.ConfigureMoney("Amount", "Currency"));

        builder.HasIndex(p => p.PaymentDate);

        builder.Ignore(p => p.DomainEvents);
    }
}