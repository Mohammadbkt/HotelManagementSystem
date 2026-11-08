using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Guest;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;

namespace HotelManagementSystem.Domain.Entities.Guests;

public sealed class Guest : SoftDeletableEntity
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public EmailAddress? Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public Guid? UserId { get; private set; }
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

        if (email is null && phoneNumber is null)
            return Result<Guest>.Failure(GuestErrors.AtLeastOneContactRequired);


        if (string.IsNullOrWhiteSpace(firstName))
            return Result<Guest>.Failure(GuestErrors.FirstNameRequired);

        if (firstName.Trim().Length > 50)
            return Result<Guest>.Failure(GuestErrors.FirstNameTooLong);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<Guest>.Failure(GuestErrors.LastNameRequired);

        if (lastName.Trim().Length > 50)
            return Result<Guest>.Failure(GuestErrors.LastNameTooLong);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (dateOfBirth > today || dateOfBirth < today.AddYears(-150))
            return Result<Guest>.Failure(GuestErrors.InvalidDateOfBirth);

        return Result<Guest>.Success(new Guest(
            Guid.NewGuid(),
            firstName.Trim(),
            lastName.Trim(),
            email,
            phoneNumber,
            dateOfBirth));
    }

    public Result UpdateInfo(string firstName, string lastName, DateOnly dateOfBirth)
    {

        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure(GuestErrors.FirstNameRequired);

        if (firstName.Trim().Length > 50)
            return Result.Failure(GuestErrors.FirstNameTooLong);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure(GuestErrors.LastNameRequired);

        if (lastName.Trim().Length > 50)
            return Result.Failure(GuestErrors.LastNameTooLong);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (dateOfBirth > today || dateOfBirth < today.AddYears(-150))
            return Result.Failure(GuestErrors.InvalidDateOfBirth);


        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;

        return Result.Success();
    }

    public Result UpdateContactInfo(EmailAddress? email, PhoneNumber? phoneNumber)
    {

        if (email is null && phoneNumber is null)
            return Result.Failure(GuestErrors.AtLeastOneContactRequired);

        Email = email;
        PhoneNumber = phoneNumber;

        return Result.Success();
    }

    public Result LinkToUserAccount(Guid userId)
    {
        if (userId == Guid.Empty)
            return Result.Failure(GuestErrors.UserIdRequired);
        if (UserId is not null)
            return Result.Failure(GuestErrors.AlreadyLinkedToAccount);

        UserId = userId;
        return Result.Success();
    }

    public override void Delete(Guid deletedBy)
    {
        if (IsDeleted) return;
        base.Delete(deletedBy);
        AddDomainEvent(new GuestDeletedDomainEvent(Id, UserId, deletedBy));
    }
}