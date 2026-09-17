using FluentValidation;

namespace HotelManagementSystem.Application.Features.Billing.Commands.AddInvoiceLineItem
{
    public class AddInvoiceLineItemCommandValidator : AbstractValidator<AddInvoiceLineItemCommand>
    {
        public AddInvoiceLineItemCommandValidator()
        {
            RuleFor(x => x.InvoiceId)
               .NotEmpty()
               .WithMessage("Invoice id is required.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x.UnitPriceAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Unit price amount cannot be negative.");

            RuleFor(x => x.UnitPriceCurrency)
                .NotEmpty()
                .WithMessage("Unit price currency is required.")
                .Length(3)
                .WithMessage("Currency must be a 3-letter ISO code.");
        }
    }
}