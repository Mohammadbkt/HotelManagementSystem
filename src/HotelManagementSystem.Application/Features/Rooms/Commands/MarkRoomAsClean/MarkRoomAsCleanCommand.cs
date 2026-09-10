using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.MarkRoomAsClean
{
    public sealed record MarkRoomAsCleanCommand(
        Guid RoomId
    ) : IRequest<Result>;
    
}