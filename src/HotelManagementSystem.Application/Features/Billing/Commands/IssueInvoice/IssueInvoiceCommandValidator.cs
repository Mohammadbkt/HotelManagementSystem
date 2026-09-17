using FluentValidation;

namespace HotelManagementSystem.Application.Features.Billing.Commands.IssueInvoice
{
    public sealed class IssueInvoiceCommandValidator : AbstractValidator<IssueInvoiceCommand>
    {
        public IssueInvoiceCommandValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty();
        }
    }
}