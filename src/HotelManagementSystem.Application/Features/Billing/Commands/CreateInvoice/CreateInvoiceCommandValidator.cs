using FluentValidation;

namespace HotelManagementSystem.Application.Features.Billing.Commands.CreateInvoice
{
    public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInvoiceCommandValidator()
        {
            RuleFor(x => x.StayId)
                .NotEmpty()
                .WithMessage("Stay ID is required.");

            RuleFor(x => x.GuestId)
                .NotEmpty()
                .WithMessage("Guest ID is required.");

            RuleFor(x => x.CurrencyCode)
                .NotEmpty()
                .WithMessage("Currency code is required.")
                .Length(3)
                .WithMessage("Currency code must be exactly 3 characters.")
                .Matches("^[A-Za-z]{3}$")
                .WithMessage("Currency code must contain only letters.")
                .Must(code => code == code.ToUpperInvariant())
                .WithMessage("Currency code must be uppercase.");
        }
    }
}