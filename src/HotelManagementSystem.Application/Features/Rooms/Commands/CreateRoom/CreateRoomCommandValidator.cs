using FluentValidation;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.CreateRoom
{
    public sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
    {
        public CreateRoomCommandValidator()
        {
            RuleFor(x => x.HotelId)
                .NotEmpty();

            RuleFor(x => x.RoomNumber)
                .NotEmpty();

            RuleFor(x => x.RoomCategoryId)
                .NotEmpty();
        }
    }
}