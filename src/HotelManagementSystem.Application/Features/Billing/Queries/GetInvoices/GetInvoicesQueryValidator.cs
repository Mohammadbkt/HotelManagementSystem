using FluentValidation;
using HotelManagementSystem.Domain.Entities.Billing;

namespace HotelManagementSystem.Application.Features.Billing.Queries.GetInvoices;

public sealed class GetInvoicesQueryValidator
    : AbstractValidator<GetInvoicesQuery>
{
    public GetInvoicesQueryValidator()
    {
        RuleFor(x => x.HotelId)
            .NotEqual(Guid.Empty)
            .When(x => x.HotelId.HasValue)
            .WithMessage("HotelId cannot be an empty GUID.");

        RuleFor(x => x.StayId)
            .NotEqual(Guid.Empty)
            .When(x => x.StayId.HasValue)
            .WithMessage("StayId cannot be an empty GUID.");

        RuleFor(x => x.SortDirection)
            .IsInEnum()
            .WithMessage("SortDirection must be a valid sort direction.");

        RuleFor(x => x.SortBy)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
            .WithMessage("SortBy cannot exceed 50 characters.");

        RuleFor(x => x.IssuedFrom)
            .LessThanOrEqualTo(x => x.IssuedTo)
            .When(x => x.IssuedFrom.HasValue && x.IssuedTo.HasValue)
            .WithMessage("IssuedFrom must be earlier than or equal to IssuedTo.");

        RuleFor(x => x.PaidFrom)
            .LessThanOrEqualTo(x => x.PaidTo)
            .When(x => x.PaidFrom.HasValue && x.PaidTo.HasValue)
            .WithMessage("PaidFrom must be earlier than or equal to PaidTo.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Status must be a valid invoice status.");

        RuleFor(x => x.AmountFrom)
            .GreaterThanOrEqualTo(0)
            .When(x => x.AmountFrom.HasValue)
            .WithMessage("AmountFrom must be greater than or equal to zero.");

        RuleFor(x => x.AmountTo)
            .GreaterThanOrEqualTo(0)
            .When(x => x.AmountTo.HasValue)
            .WithMessage("AmountTo must be greater than or equal to zero.");

        RuleFor(x => x.AmountFrom)
            .LessThanOrEqualTo(x => x.AmountTo)
            .When(x => x.AmountFrom.HasValue && x.AmountTo.HasValue)
            .WithMessage("AmountFrom must be less than or equal to AmountTo.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}