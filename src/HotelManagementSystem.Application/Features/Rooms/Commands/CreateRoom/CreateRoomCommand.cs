using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.CreateRoom
{
    public sealed record CreateRoomCommand(
        Guid HotelId,
        string RoomNumber,
        Guid RoomCategoryId
    ) : IRequest<Result<Guid>>;
    
}