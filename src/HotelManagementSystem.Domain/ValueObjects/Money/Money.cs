using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.ValueObjects.Money
{
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
            if (amount < 0)
                return MoneyErrors.AmountNegative;

            if (string.IsNullOrWhiteSpace(currency))
                return MoneyErrors.CurrencyRequired;

            var formattedCurrency = currency.Trim().ToUpperInvariant();

            if (formattedCurrency.Length != 3)
                return MoneyErrors.InvalidCurrency;

            return new Money(decimal.Round(amount, 2), formattedCurrency);
        }

        public static Money Zero(string currency = "USD") => new(0m, currency.ToUpperInvariant());

        #region Domain Operations

        public Result<Money> Add(Money other)
        {
            if (Currency != other.Currency)
                return MoneyErrors.CurrencyMismatch;

            return new Money(Amount + other.Amount, Currency);
        }

        public Result<Money> Subtract(Money other)
        {
            if (Currency != other.Currency)
                return MoneyErrors.CurrencyMismatch;

            if (Amount - other.Amount < 0)
                return MoneyErrors.AmountNegative;

            return new Money(Amount - other.Amount, Currency);
        }

        public static Result<Money> operator +(Money left, Money right) => left.Add(right);
        public static Result<Money> operator -(Money left, Money right) => left.Subtract(right);

        #endregion
    }
}