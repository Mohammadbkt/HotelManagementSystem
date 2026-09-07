using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;

namespace HotelManagementSystem.Domain.Entities.Guests;

public sealed class Guest : AuditableEntity
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public EmailAddress? Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateOnly DateOfBirth { get; private set; }

    private Guest() { }

    private Guest(
        Guid id,
        string firstName,
        string lastName,
        EmailAddress? email,
        PhoneNumber? phoneNumber,
        DateOnly dateOfBirth) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
    }

    public static Result<Guest> Create(
        string firstName,
        string lastName,
        EmailAddress? email,
        PhoneNumber? phoneNumber,
        DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (string.IsNullOrWhiteSpace(firstName))
            return Result<Guest>.Failure(GuestErrors.FirstNameRequired);

        if (firstName.Trim().Length > 50)
            return Result<Guest>.Failure(GuestErrors.FirstNameTooLong);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<Guest>.Failure(GuestErrors.LastNameRequired);

        if (lastName.Trim().Length > 50)
            return Result<Guest>.Failure(GuestErrors.LastNameTooLong);

        if (dateOfBirth > today)
            return Result<Guest>.Failure(GuestErrors.InvalidDateOfBirth);

        if (dateOfBirth < today.AddYears(-150))
            return Result<Guest>.Failure(GuestErrors.InvalidDateOfBirth);

        return Result<Guest>.Success(new Guest(
            Guid.NewGuid(),
            firstName.Trim(),
            lastName.Trim(),
            email,
            phoneNumber,
            dateOfBirth));
    }

    public Result UpdateName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure(GuestErrors.FirstNameRequired);

        if (firstName.Trim().Length > 50)
            return Result.Failure(GuestErrors.FirstNameTooLong);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure(GuestErrors.LastNameRequired);

        if (lastName.Trim().Length > 50)
            return Result.Failure(GuestErrors.LastNameTooLong);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();

        return Result.Success();
    }

    public Result UpdateContactInfo(EmailAddress? email, PhoneNumber? phoneNumber)
    {
        Email = email;
        PhoneNumber = phoneNumber;

        return Result.Success();
    }
}