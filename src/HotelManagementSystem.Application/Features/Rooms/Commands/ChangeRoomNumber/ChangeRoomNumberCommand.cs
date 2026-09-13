using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomNumber
{
    public sealed record ChangeRoomNumberCommand(
        Guid RoomId,
        string RoomNumber
    ) : IRequest<Result>;
}