using FluentValidation;

namespace HotelManagementSystem.Application.Features.Billing.Commands.PayInvoice
{
    public sealed class PayInvoiceCommandValidator : AbstractValidator<PayInvoiceCommand>
    {
        public PayInvoiceCommandValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty();
        }
    }
}