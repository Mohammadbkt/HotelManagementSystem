using FluentValidation;

namespace HotelManagementSystem.Application.Features.Billing.Queries.GetInvoiceById
{
    public sealed class GetInvoiceByIdQueryValidator : AbstractValidator<GetInvoiceByIdQuery>
    {
        public GetInvoiceByIdQueryValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty();
        }
    }
}