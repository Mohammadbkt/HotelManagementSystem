using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ReturnRoomToService
{
    public sealed record ReturnRoomToServiceCommand(
        Guid RoomId
    ) : IRequest<Result>;
}