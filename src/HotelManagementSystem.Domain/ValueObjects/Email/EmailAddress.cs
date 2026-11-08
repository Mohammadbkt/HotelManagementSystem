using System;
using System.Text.RegularExpressions;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.ValueObjects.Email
{
    public sealed record EmailAddress
    {
        private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(100));

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

            if (!EmailRegex.IsMatch(value))
            {
                return Result<EmailAddress>.Failure(EmailAddressErrors.InvalidFormat);
            }

            var normalizedValue = value.ToLowerInvariant();

            return Result<EmailAddress>.Success(new EmailAddress(normalizedValue));
        }

        public override string ToString() => Value;
    }
}
