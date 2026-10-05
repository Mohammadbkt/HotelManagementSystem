using HotelManagementSystem.Domain.ValueObjects.Money;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Persistence.Configurations;

internal static class MoneyConfigurationExtensions
{
    
    public static ComplexPropertyBuilder<Money> ConfigureMoney(
        this ComplexPropertyBuilder<Money> builder,
        string amountColumn,
        string currencyColumn)
    {
        builder.Property(m => m.Amount)
            .HasColumnName(amountColumn)
            .HasPrecision(18, 2);

        builder.Property(m => m.Currency)
            .HasColumnName(currencyColumn)
            .HasMaxLength(3)
            .IsFixedLength();

        return builder;
    }
}