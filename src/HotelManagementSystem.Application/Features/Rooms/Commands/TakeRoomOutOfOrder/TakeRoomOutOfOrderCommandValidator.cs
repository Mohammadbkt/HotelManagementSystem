using FluentValidation;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.TakeRoomOutOfOrder
{
    public sealed class TakeRoomOutOfOrderCommandValidator : AbstractValidator<TakeRoomOutOfOrderCommand>
    {
        public TakeRoomOutOfOrderCommandValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty();

                
            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Reason));
        }
    }
}