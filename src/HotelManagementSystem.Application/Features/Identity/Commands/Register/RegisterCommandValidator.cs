using FluentValidation;
using HotelManagementSystem.Application.Features.Identity.Commands.Register;

namespace HotelManagementSystem.Application.Validators.Identity;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(30)
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Username can only contain letters, numbers, and underscores.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Password must contain uppercase, lowercase, and number");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[\d\s-]{10,}$").When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Invalid phone number format");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty();
    }
}