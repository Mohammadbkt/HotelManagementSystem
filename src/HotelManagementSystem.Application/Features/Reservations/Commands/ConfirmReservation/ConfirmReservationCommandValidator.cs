using FluentValidation;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.ConfirmReservation
{
    public class ConfirmReservationCommandValidator : AbstractValidator<ConfirmReservationCommand>
    {
        public ConfirmReservationCommandValidator()
        {
            RuleFor(x => x.ReservationId)
                .NotEmpty();
        }
    }
}