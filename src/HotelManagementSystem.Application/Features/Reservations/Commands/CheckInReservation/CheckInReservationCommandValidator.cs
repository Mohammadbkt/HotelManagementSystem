using FluentValidation;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CheckInReservation
{
    public class CheckInReservationCommandValidator : AbstractValidator<CheckInReservationCommand>
    {
        public CheckInReservationCommandValidator()
        {
            RuleFor(x => x.ReservationId)
                .NotEmpty();
        }
    }
}