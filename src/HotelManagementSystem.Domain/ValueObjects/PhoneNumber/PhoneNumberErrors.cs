using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.ValueObjects.PhoneNumber
{
    public static class PhoneNumberErrors
    {
        public static readonly Error Empty =
            Error.Validation(
                "PhoneNumber.Empty",
                "Phone number cannot be empty.");

        public static readonly Error InvalidFormat =
            Error.Validation(
                "PhoneNumber.InvalidFormat",
                "Phone number format is invalid.");

        public static readonly Error InvalidLength =
            Error.Validation(
                "PhoneNumber.InvalidLength",
                "Phone number must contain between 7 and 15 digits.");

        public static readonly Error MissingCountryCode =
            Error.Validation(
                "PhoneNumber.MissingCountryCode",
                "Phone number must include a country calling code.");
    }
}
