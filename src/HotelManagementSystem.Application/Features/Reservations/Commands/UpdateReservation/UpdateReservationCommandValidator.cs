using FluentValidation;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.UpdateReservation
{
    public sealed class UpdateReservationCommandValidator : AbstractValidator<UpdateReservationCommand>
    {
        public UpdateReservationCommandValidator()
        {
            RuleFor(x => x.ReservationId)
                .NotEmpty()
                .WithMessage("Reservation ID is required.");

            RuleFor(x => x.RoomId)
                .NotEmpty()
                .WithMessage("Room ID is required.");

            RuleFor(x => x.CheckInDate)
                .NotEmpty()
                .WithMessage("Check-in date is required.")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Check-in date cannot be in the past.");

            RuleFor(x => x.CheckOutDate)
                .NotEmpty()
                .WithMessage("Check-out date is required.")
                .GreaterThan(x => x.CheckInDate)
                .WithMessage("Check-out date must be after check-in date.");

            RuleFor(x => x)
                .Must(x => x.CheckOutDate.DayNumber - x.CheckInDate.DayNumber >= 1)
                .WithMessage("Reservation must be for at least one night.")
                .When(x => x.CheckInDate != default && x.CheckOutDate != default);

            RuleFor(x => x)
                .Must(x => x.CheckOutDate.DayNumber - x.CheckInDate.DayNumber <= 30)
                .WithMessage("Reservation cannot exceed 30 nights.")
                .When(x => x.CheckInDate != default && x.CheckOutDate != default);
        }
    }
}