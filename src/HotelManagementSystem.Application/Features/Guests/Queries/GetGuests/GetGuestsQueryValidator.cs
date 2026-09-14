using FluentValidation;

namespace HotelManagementSystem.Application.Features.Guests.Queries.GetGuests
{
    public sealed class GetGuestsQueryValidator : AbstractValidator<GetGuestsQuery>
    {
        public GetGuestsQueryValidator()
        {
            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

        }

        
    }
}
