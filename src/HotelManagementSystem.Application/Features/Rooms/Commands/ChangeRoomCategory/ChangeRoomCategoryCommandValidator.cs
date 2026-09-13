using FluentValidation;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomCategory
{
    public sealed class ChangeRoomCategoryCommandValidator : AbstractValidator<ChangeRoomCategoryCommand>
    {
        public ChangeRoomCategoryCommandValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty();

            RuleFor(x => x.RoomCategoryId)
                .NotEmpty();
        }
    }
}