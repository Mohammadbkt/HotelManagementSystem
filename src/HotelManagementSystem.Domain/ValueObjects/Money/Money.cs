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

    public static Result<Money> Create(
        decimal amount,
        string currency)
    {
        if (amount < 0)
            return Result<Money>.Failure(MoneyErrors.AmountNegative);

        if (string.IsNullOrWhiteSpace(currency))
            return Result<Money>.Failure(MoneyErrors.CurrencyRequired);

        var formattedCurrency = currency.Trim().ToUpperInvariant();

        if (formattedCurrency.Length != 3)
            return Result<Money>.Failure(MoneyErrors.InvalidCurrency);

        return Result<Money>.Success(new Money(
            decimal.Round(amount, 2),
            formattedCurrency));
    }

    public static Money Zero(string currency = "JOD")
    {
        var formattedCurrency = currency.Trim().ToUpperInvariant();

        return new Money(0m, formattedCurrency);
    }

    public Result<Money> Add(Money other)
    {
        if (Currency != other.Currency)
            return Result<Money>.Failure(MoneyErrors.CurrencyMismatch);

        return Result<Money>.Success(new Money(
            decimal.Round(Amount + other.Amount, 2),
            Currency));
    }

    public Result<Money> Subtract(Money other)
    {
        if (Currency != other.Currency)
            return Result<Money>.Failure(MoneyErrors.CurrencyMismatch);

        if (Amount < other.Amount)
            return Result<Money>.Failure(MoneyErrors.AmountNegative);

        return Result<Money>.Success(new Money(
            decimal.Round(Amount - other.Amount, 2),
            Currency));
    }

    public Result<Money> Multiply(int quantity)
    {
        if (quantity < 0)
            return Result<Money>.Failure(MoneyErrors.AmountNegative);

        return Result<Money>.Success(new Money(
            decimal.Round(Amount * quantity, 2),
            Currency));
    }

    public override string ToString()
        => $"{Amount:0.00} {Currency}";
}