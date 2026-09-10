using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.MoveRoomToMaintenance
{
    public sealed record MoveRoomToMaintenanceCommand(
        Guid RoomId,
        string? Reason
    ) : IRequest<Result>;
}