using FluentValidation;
using HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuest;

namespace HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuestInfo
{
    public sealed class UpdateGuestInfoCommandValidator : AbstractValidator<UpdateGuestInfoCommand>
    {
        public UpdateGuestInfoCommandValidator()
        {
            RuleFor(x => x.GuestId)
                .NotEmpty()
                .WithMessage("GuestId is required.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(50)
                .WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(50)
                .WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.DateOfBirth)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-150))
                .WithMessage("Date of birth is not plausible.");
        }
    }
}