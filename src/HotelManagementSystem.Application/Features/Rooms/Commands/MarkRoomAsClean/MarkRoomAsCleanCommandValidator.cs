using FluentValidation;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.MarkRoomAsClean
{
    public sealed class MarkRoomAsCleanCommandValidator : AbstractValidator<MarkRoomAsCleanCommand>
    {
        public MarkRoomAsCleanCommandValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty();
        }
    }
}