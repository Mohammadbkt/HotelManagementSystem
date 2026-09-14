using FluentValidation;
using HotelManagementSystem.Domain.Entities.Reservations;

namespace HotelManagementSystem.Application.Features.Reservations.Queries.GetReservations
{
    public sealed class GetReservationsQueryValidator
        : AbstractValidator<GetReservationsQuery>
    {
        public GetReservationsQueryValidator()
        {
            RuleFor(x => x.GuestId)
                .NotEqual(Guid.Empty)
                .When(x => x.GuestId.HasValue)
                .WithMessage("Guest ID must be a valid non-empty GUID.");

            RuleFor(x => x.RoomId)
                .NotEqual(Guid.Empty)
                .When(x => x.RoomId.HasValue)
                .WithMessage("Room ID must be a valid non-empty GUID.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue)
                .WithMessage("Invalid reservation status.");

            RuleFor(x => x.CheckInDate)
                .NotEqual(default(DateOnly))
                .When(x => x.CheckInDate.HasValue)
                .WithMessage("Check-in date must be a valid date.");

            RuleFor(x => x.CheckOutDate)
                .NotEqual(default(DateOnly))
                .When(x => x.CheckOutDate.HasValue)
                .WithMessage("Check-out date must be a valid date.");

            RuleFor(x => x)
                .Must(x => !x.CheckInDate.HasValue
                        || !x.CheckOutDate.HasValue
                        || x.CheckInDate <= x.CheckOutDate)
                .WithMessage("Check-in date must be before or equal to check-out date.")
                .OverridePropertyName(nameof(GetReservationsQuery.CheckInDate));

            RuleFor(x => x.SortDirection)
                .IsInEnum()
                .WithMessage("Invalid sort direction.");

            
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");
        }
    }
}