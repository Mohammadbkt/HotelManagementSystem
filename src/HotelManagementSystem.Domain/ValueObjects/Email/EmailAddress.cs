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
                return Result<EmailAddress>.Failure(EmailAddressErrors.Empty);
            }

            value = value.Trim();

            if (value.Length > 254)
            {
                return Result<EmailAddress>.Failure(EmailAddressErrors.TooLong);
            }

            if (!Regex.IsMatch(
                    value,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.CultureInvariant))
            {
                return Result<EmailAddress>.Failure(EmailAddressErrors.InvalidFormat);
            }

            var normalizedValue = value.ToLowerInvariant();

            return Result<EmailAddress>.Success(new EmailAddress(normalizedValue));
        }

        public override string ToString() => Value;
    }
}
