using FluentValidation;

namespace HotelManagementSystem.Application.Features.Stays.Commands.CheckInGuest
{
    public sealed class CheckInGuestCommandValidator : AbstractValidator<CheckInGuestCommand>
    {
        public CheckInGuestCommandValidator()
        {
            When(x => x.ReservationId is null, () =>
            {
                RuleFor(x => x.GuestId)
                    .NotEmpty()
                    .WithMessage("Guest ID is required for a walk-in check-in.");

                RuleFor(x => x.RoomId)
                    .NotEmpty()
                    .WithMessage("Room ID is required for a walk-in check-in.");
            });

            When(x => x.ReservationId is not null, () =>
            {
                RuleFor(x => x.ReservationId)
                    .NotEqual(Guid.Empty)
                    .WithMessage("Reservation ID cannot be empty.");
            });

            RuleFor(x => x.ExpectedCheckOutDate)
                .NotNull().WithMessage("Expected check-out date is required for walk-in check-in.")
                .When(x => x.ReservationId is null);

            RuleFor(x => x.ExpectedCheckOutDate)
                .Must(d => d!.Value > DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Expected check-out date must be after today.")
                .When(x => x.ReservationId is null && x.ExpectedCheckOutDate is not null);
        }
    }
}