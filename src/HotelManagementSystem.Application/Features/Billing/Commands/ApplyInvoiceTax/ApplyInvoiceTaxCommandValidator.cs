using FluentValidation;
using HotelManagementSystem.Application.Features.Billing.Commands.ApplyInvoiceTax;

namespace HotelManagementSystem.Application.Features.Invoices.Commands.ApplyInvoiceTax
{
    public sealed class ApplyInvoiceTaxCommandValidator : AbstractValidator<ApplyInvoiceTaxCommand>
    {
        public ApplyInvoiceTaxCommandValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty()
                .WithMessage("Invoice ID is required.");

            RuleFor(x => x.TaxAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Tax amount cannot be negative.");

            RuleFor(x => x.TaxCurrency)
                .NotEmpty()
                .WithMessage("Currency code is required.")
                .Length(3)
                .WithMessage("Currency code must be a 3-letter ISO code (e.g., JOD, USD).");
        }
    }
}
