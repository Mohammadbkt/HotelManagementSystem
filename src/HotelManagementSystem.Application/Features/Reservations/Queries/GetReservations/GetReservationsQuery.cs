using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Reservations.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Reservations;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Queries.GetReservations
{
    public sealed record GetReservationsQuery(
        Guid? GuestId = null,
        Guid? RoomId = null,
        ReservationStatus? Status = null,
        DateOnly? CheckInDate = null,
        DateOnly? CheckOutDate = null,
        string? SortBy = null,
        SortDirection SortDirection = SortDirection.Ascending,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<Result<PaginatedList<ReservationDto>>>;
}