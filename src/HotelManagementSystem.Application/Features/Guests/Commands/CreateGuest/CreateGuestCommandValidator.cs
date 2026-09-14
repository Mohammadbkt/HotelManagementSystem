using FluentValidation;

namespace HotelManagementSystem.Application.Features.Guests.Commands.CreateGuest
{
    public sealed class CreateGuestCommandValidator : AbstractValidator<CreateGuestCommand>
    {
        public CreateGuestCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .EmailAddress()
                .MaximumLength(254)
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(30)
                .Matches(@"^\+?[0-9\s\-()]+$")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.DateOfBith)
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date of birth must be in the past.");
        }
    }
}