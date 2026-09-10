using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.TakeRoomOutOfOrder
{
    public sealed record TakeRoomOutOfOrderCommand(
        Guid RoomId,
        string? Reason
    ) :IRequest<Result>;
}