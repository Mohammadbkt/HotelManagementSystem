using System;
using System.Linq;
using System.Text.RegularExpressions;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.ValueObjects.PhoneNumber
{
    public sealed record PhoneNumber
    {
        private static readonly Regex E164Regex = new(
        @"^\+[1-9]\d{6,14}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(100));

        public string Value { get; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static Result<PhoneNumber> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result<PhoneNumber>.Failure(PhoneNumberErrors.Empty);
            }

            value = value.Trim();

            var normalized = new string(value
            .Where(c => char.IsDigit(c) || c == '+')
            .ToArray());

            if (!E164Regex.IsMatch(normalized))
            {
                return Result<PhoneNumber>.Failure(PhoneNumberErrors.InvalidFormat);
            }
            
            return Result<PhoneNumber>.Success(new PhoneNumber(normalized));
        }

        public override string ToString() => Value;
    }
}

