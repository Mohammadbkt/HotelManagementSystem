using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.ValueObjects.Email
{
    public static class EmailAddressErrors
    {
        public static readonly Error Empty =
            Error.Validation(
                "EmailAddress.Empty",
                "Email address cannot be empty.");

        public static readonly Error InvalidFormat =
            Error.Validation(
                "EmailAddress.InvalidFormat",
                "Email address format is invalid.");

        public static readonly Error TooLong =
            Error.Validation(
                "EmailAddress.TooLong",
                "Email address cannot exceed 254 characters.");
    }
}
