using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Stays.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Stays.Queries.GetStays
{
    public sealed record GetStaysQuery(
        Guid? GuestId = null,
        Guid? RoomId = null,
        Guid? ReservationId = null,
        bool? IsActive = null,
        DateTimeOffset? CheckInFrom = null,
        DateTimeOffset? CheckInTo = null,
        DateTimeOffset? CheckOutFrom = null,
        DateTimeOffset? CheckOutTo = null,
        string? SortBy = null,
        SortDirection SortDirection = SortDirection.Ascending,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<Result<PaginatedList<StayDto>>>;
}