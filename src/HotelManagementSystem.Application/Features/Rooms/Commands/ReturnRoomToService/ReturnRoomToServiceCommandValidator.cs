using FluentValidation;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ReturnRoomToService
{
  public sealed class ReturnRoomToServiceCommandValidator : AbstractValidator<ReturnRoomToServiceCommand>
    {
        public ReturnRoomToServiceCommandValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty();


        }
    }
}