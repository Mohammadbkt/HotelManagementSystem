using FluentValidation;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomNumber
{
    public sealed class ChangeRoomNumberCommandValidator : AbstractValidator<ChangeRoomNumberCommand>
    {
        public ChangeRoomNumberCommandValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty();

            RuleFor(x => x.RoomNumber)
                .NotEmpty()
                .MaximumLength(20);
        }
    }
}