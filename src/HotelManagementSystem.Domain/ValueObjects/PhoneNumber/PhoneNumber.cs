using System;
using System.Linq;
using System.Text.RegularExpressions;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.ValueObjects.PhoneNumber
{
    public readonly record struct PhoneNumber
    {
        public string Value { get; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static Result<PhoneNumber> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return PhoneNumberErrors.Empty;
            }

            value = value.Trim();

            if (!Regex.IsMatch(value, @"^[\d\s\-\(\)\+]+$"))
            {
                return PhoneNumberErrors.InvalidFormat;
            }

            var digitsOnly = new string(
                value.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length < 7 || digitsOnly.Length > 15)
            {
                return PhoneNumberErrors.InvalidLength;
            }

            if (!value.StartsWith('+'))
            {
                return PhoneNumberErrors.MissingCountryCode;
            }

            var normalizedValue = $"+{digitsOnly}";

            return new PhoneNumber(normalizedValue);
        }

        public override string ToString() => Value;
    }
}

