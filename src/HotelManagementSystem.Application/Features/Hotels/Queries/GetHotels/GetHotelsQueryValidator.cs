using FluentValidation;

namespace HotelManagementSystem.Application.Features.Hotels.Queries.GetHotels
{
    public sealed class GetHotelsQueryValidator : AbstractValidator<GetHotelsQuery>
    {
        public GetHotelsQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size cannot exceed 100.");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
                .WithMessage("Search term cannot exceed 100 characters.");

            RuleFor(x => x.City)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.Country)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Country))
                .WithMessage("Country cannot exceed 100 characters.");

            RuleFor(x => x.SortBy)
                .Must(s => s is null || s.Equals("name", StringComparison.OrdinalIgnoreCase) 
                                     || s.Equals("city", StringComparison.OrdinalIgnoreCase))
                .WithMessage("SortBy must be one of: name, city.");

            RuleFor(x => x.SortDirection)
                .IsInEnum()
                .WithMessage("SortDirection must be Ascending or Descending.");

        }
    }
}