using FluentValidation;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CancelReservation
{
    public class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand>
    {
        public CancelReservationCommandValidator()
        {
            RuleFor(x => x.ReservationId)
                .NotEmpty();
        }
    }
}