using HotelManagementSystem.Application.Features.Rooms.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Queries.GetRoomById
{
    public sealed record GetRoomByIdQuery(
        Guid RoomId
    ) : IRequest<Result<RoomDto>>;
}