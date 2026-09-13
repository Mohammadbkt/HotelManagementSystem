using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Rooms.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Rooms;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Queries.GetRooms
{
    public sealed record GetRoomsQuery(
        Guid HotelId,
        string? SearchTerm = null,
        RoomStatus? Status = null,
        string? SortBy = null,
        SortDirection SortDirection = SortDirection.Ascending,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<Result<PaginatedList<RoomDto>>>;
}