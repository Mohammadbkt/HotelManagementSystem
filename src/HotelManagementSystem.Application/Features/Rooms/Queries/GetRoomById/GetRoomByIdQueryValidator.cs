using FluentValidation;

namespace HotelManagementSystem.Application.Features.Rooms.Queries.GetRoomById
{
    public sealed class GetRoomByIdQueryValidator : AbstractValidator<GetRoomByIdQuery>
    {
        public GetRoomByIdQueryValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty();
        }
    }
}