using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.ValueObjects.Address
{
    public static class AddressErrors
    {
        public static readonly Error AddressLine1Empty =
            Error.Validation(
                "Address.AddressLine1Empty",
                "Address line 1 cannot be empty.");

        public static readonly Error AddressLine1TooLong =
            Error.Validation(
                "Address.AddressLine1TooLong",
                "Address line 1 cannot exceed 200 characters.");

        public static readonly Error AddressLine2TooLong =
            Error.Validation(
                "Address.AddressLine2TooLong",
                "Address line 2 cannot exceed 200 characters.");

        public static readonly Error CityEmpty =
            Error.Validation(
                "Address.CityEmpty",
                "City cannot be empty.");

        public static readonly Error CityTooLong =
            Error.Validation(
                "Address.CityTooLong",
                "City cannot exceed 100 characters.");

        public static readonly Error StateOrProvinceTooLong =
            Error.Validation(
                "Address.StateOrProvinceTooLong",
                "State or province cannot exceed 100 characters.");

        public static readonly Error PostalCodeTooLong =
            Error.Validation(
                "Address.PostalCodeTooLong",
                "Postal code cannot exceed 20 characters.");

        public static readonly Error CountryEmpty =
            Error.Validation(
                "Address.CountryEmpty",
                "Country cannot be empty.");

        public static readonly Error CountryTooLong =
            Error.Validation(
                "Address.CountryTooLong",
                "Country cannot exceed 100 characters.");
    }
}
