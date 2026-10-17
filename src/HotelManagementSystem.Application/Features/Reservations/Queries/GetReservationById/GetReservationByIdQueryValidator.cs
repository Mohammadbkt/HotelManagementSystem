using FluentValidation;

namespace HotelManagementSystem.Application.Features.Reservations.Queries.GetReservationById
{
    public sealed class GetReservationByIdQueryValidator
        : AbstractValidator<GetReservationByIdQuery>
    {
        public GetReservationByIdQueryValidator()
        {
            RuleFor(x => x.ReservationId)
                .NotEmpty()
                .WithMessage("Reservation ID is required.");
        }
    }
}