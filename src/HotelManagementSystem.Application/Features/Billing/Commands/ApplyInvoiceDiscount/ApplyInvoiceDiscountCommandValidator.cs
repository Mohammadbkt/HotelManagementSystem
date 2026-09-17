using FluentValidation;

namespace HotelManagementSystem.Application.Features.Billing.Commands.ApplyInvoiceDiscount
{
    public class ApplyInvoiceDiscountCommandValidator : AbstractValidator<ApplyInvoiceDiscountCommand>
    {
        public ApplyInvoiceDiscountCommandValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty()
                .WithMessage("Invoice ID is required.");

            RuleFor(x => x.DiscountAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Discount amount cannot be negative.");

            RuleFor(x => x.DiscountCurrency)
                .NotEmpty()
                .WithMessage("Currency code is required.")
                .Length(3)
                .WithMessage("Currency code must be a 3-letter ISO code (e.g., JOD, USD).");
        }
    }
}