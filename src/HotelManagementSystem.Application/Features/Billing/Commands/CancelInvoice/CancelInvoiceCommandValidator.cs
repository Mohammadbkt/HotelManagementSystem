using FluentValidation;

namespace HotelManagementSystem.Application.Features.Billing.Commands.CancelInvoice
{
    public sealed class CancelInvoiceCommandValidator : AbstractValidator<CancelInvoiceCommand>
    {
        public CancelInvoiceCommandValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty();
        }
    }
} 