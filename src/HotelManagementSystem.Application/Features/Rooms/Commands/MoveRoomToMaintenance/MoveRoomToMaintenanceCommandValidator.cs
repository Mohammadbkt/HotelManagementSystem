using FluentValidation;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.MoveRoomToMaintenance
{
    public sealed class MoveRoomToMaintenanceCommandValidator : AbstractValidator<MoveRoomToMaintenanceCommand>
    {
        public MoveRoomToMaintenanceCommandValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty();

            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Reason));

        }
    }
}