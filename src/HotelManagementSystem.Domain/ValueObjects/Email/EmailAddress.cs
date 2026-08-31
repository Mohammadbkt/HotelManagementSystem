using System;
using System.Text.RegularExpressions;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.ValueObjects.Email
{
    public readonly record struct EmailAddress
    {
        public string Value { get; }

        private EmailAddress(string value)
        {
            Value = value;
        }

        public static Result<EmailAddress> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return EmailAddressErrors.Empty;
            }

            value = value.Trim();

            if (value.Length > 254)
            {
                return EmailAddressErrors.TooLong;
            }

            if (!Regex.IsMatch(
                    value,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.CultureInvariant))
            {
                return EmailAddressErrors.InvalidFormat;
            }

            var normalizedValue = value.ToLowerInvariant();

            return new EmailAddress(normalizedValue);
        }

        public override string ToString() => Value;
    }
}
