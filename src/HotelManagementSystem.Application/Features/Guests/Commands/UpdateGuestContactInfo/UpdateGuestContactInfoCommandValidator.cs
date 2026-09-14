using FluentValidation;

namespace HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuestContactInfo
{
    public sealed class UpdateGuestContactInfoCommandValidator : AbstractValidator<UpdateGuestContactInfoCommand>
    {
        public UpdateGuestContactInfoCommandValidator()
        {
            RuleFor(x => x.GuestId)
                .NotEmpty()
                .WithMessage("GuestId is required.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Email is not a valid email address.")
                .MaximumLength(254)
                .WithMessage("Email cannot exceed 254 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(30)
                .WithMessage("Phone number cannot exceed 30 characters.")
                .Matches(@"^\+?[0-9\s\-()]+$")
                .WithMessage("Phone number contains invalid characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));
        }
    }
}