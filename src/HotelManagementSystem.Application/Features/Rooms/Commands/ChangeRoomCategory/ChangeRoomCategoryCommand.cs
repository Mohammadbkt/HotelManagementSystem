using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomCategory
{
    public sealed record ChangeRoomCategoryCommand(
        Guid RoomId,
        Guid RoomCategoryId
    ) : IRequest<Result>;
}