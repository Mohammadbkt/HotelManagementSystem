using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Guests;

public static class GuestErrors
{
    public static Error FirstNameRequired => Error.Validation(
        "Guest.FirstNameRequired",
        "First name is required.");

    public static Error FirstNameTooLong => Error.Validation(
        "Guest.FirstNameTooLong",
        "First name cannot exceed 50 characters.");

    public static Error LastNameRequired => Error.Validation(
        "Guest.LastNameRequired",
        "Last name is required.");

    public static Error LastNameTooLong => Error.Validation(
        "Guest.LastNameTooLong",
        "Last name cannot exceed 50 characters.");

    public static Error InvalidDateOfBirth => Error.Validation(
        "Guest.InvalidDateOfBirth",
        "Date of birth is invalid.");

    public static Error AtLeastOneContactRequired => Error.Validation(
        "Guest.AtLeastOneContactRequired",
        "At least one contact method (email or phone) is required.");

    public static Error AlreadyLinkedToAccount => Error.Validation(
        "Guest.AlreadyLinkedToAccount",
        "Guest already linked to account.");

    public static Error NotFound => 
    Error.NotFound(
        "Guest.NotFound",
        " guest not found.");

    public static Error AmbiguousMatch =>
    Error.Conflict(
        "Guest.AmbiguousMatch",
        "Multiple guests found with the same email or phone number.");
}