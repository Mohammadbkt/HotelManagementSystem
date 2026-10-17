using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.ValueObjects.Money;

public readonly record struct Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            return Result<Money>.Failure(MoneyErrors.CurrencyRequired);

        if (amount < 0)
            return Result<Money>.Failure(MoneyErrors.AmountNegative);

        var formattedCurrency = currency.Trim().ToUpperInvariant();

        if (formattedCurrency.Length != 3)
            return Result<Money>.Failure(MoneyErrors.InvalidCurrency);

        return Result<Money>.Success(new Money(amount, formattedCurrency));
    }

    public static Money FromTrustedSource(decimal amount, string normalizedCurrency)
        => new(amount, normalizedCurrency);

    public static Money Zero(string currency)
    {
        var formattedCurrency = currency.Trim().ToUpperInvariant();

        return new Money(0m, formattedCurrency);
    }

    public Result<Money> Add(Money other)
    {
        if (Currency != other.Currency)
            return Result<Money>.Failure(MoneyErrors.CurrencyMismatch);

        return Result<Money>.Success(new Money(Amount + other.Amount, Currency));
    }

    public Result<Money> Subtract(Money other)
    {
        if (Currency != other.Currency)
            return Result<Money>.Failure(MoneyErrors.CurrencyMismatch);

        return Result<Money>.Success(new Money(Amount - other.Amount, Currency));
    }

    public Result<Money> Multiply(int quantity)
    {
        return Result<Money>.Success(new Money(
            decimal.Round(Amount * quantity, 2),
            Currency));
    }

    public Result<Money> Devide(int divisor)
    {
        if (divisor == 0m)
        {
            return Result<Money>.Failure(MoneyErrors.DivideByZero);
        }

        return Result<Money>.Success(new Money(Amount / divisor, Currency));
    }

    public Money Rounded(int decimals = 2, MidpointRounding mode = MidpointRounding.ToEven)
        => new(decimal.Round(Amount, decimals, mode), Currency);

    public bool IsZero => Amount == 0m;
    public bool IsPositive => Amount > 0m;
    public bool IsNegative => Amount < 0m;

    public bool Equals(Money other)
        => Amount == other.Amount
        && string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode()
        => HashCode.Combine(Amount, Currency.ToUpperInvariant());

    public override string ToString()
        => $"{Amount:0.00} {Currency}";
}