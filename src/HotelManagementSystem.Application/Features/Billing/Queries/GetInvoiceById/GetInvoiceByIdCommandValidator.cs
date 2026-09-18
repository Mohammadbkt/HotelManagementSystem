using FluentValidation;

namespace HotelManagementSystem.Application.Features.Billing.Queries.GetInvoiceById
{
    public sealed class GetInvoiceByIdCommandValidator : AbstractValidator<GetInvoiceByIdCommand>
    {
        public GetInvoiceByIdCommandValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty();
        }
    }
}