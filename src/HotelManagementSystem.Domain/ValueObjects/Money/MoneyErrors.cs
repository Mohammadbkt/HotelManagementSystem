using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.ValueObjects.Money
{
    public static class MoneyErrors
    {
        public static readonly Error AmountNegative =
            Error.Validation("Money.AmountNegative", "Money amount cannot be negative.");

        public static readonly Error CurrencyRequired =
            Error.Validation("Money.CurrencyRequired", "Currency code is required.");

        public static readonly Error InvalidCurrency =
            Error.Validation("Money.InvalidCurrency", "Currency must be a 3-letter ISO code (e.g., USD, EUR).");

        public static readonly Error CurrencyMismatch =
            Error.Validation("Money.CurrencyMismatch", "Cannot perform operations on money with different currencies.");

        public static readonly Error DivideByZero = 
            Error.Validation("Money.DivideByZero", "Cannot devide by zero.");

    }
}