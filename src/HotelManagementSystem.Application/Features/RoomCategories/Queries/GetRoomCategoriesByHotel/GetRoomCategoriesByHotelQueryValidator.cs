using FluentValidation;

namespace HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoriesByHotel
{
    public class GetRoomCategoriesByHotelQueryValidator : AbstractValidator<GetRoomCategoriesByHotelQuery>
    {
        public GetRoomCategoriesByHotelQueryValidator()
        {
            RuleFor(x => x.HotelId)
                .NotEmpty();

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

            RuleFor(x => x.MaxCapacity)
                .GreaterThan(0)
                .When(x => x.MaxCapacity.HasValue);

            RuleFor(x => x.MinPricePerNight)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinPricePerNight.HasValue);

            RuleFor(x => x.MaxPricePerNight)
                .GreaterThan(0)
                .When(x => x.MaxPricePerNight.HasValue);

            RuleFor(x => x)
                .Must(x => !x.MinPricePerNight.HasValue || !x.MaxPricePerNight.HasValue ||
                          x.MinPricePerNight <= x.MaxPricePerNight)
                .WithMessage("Minimum price must be less than or equal to maximum price");

        }
    }
}