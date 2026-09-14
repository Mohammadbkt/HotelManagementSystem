using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Guests.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Guests.Queries.GetGuests
{
    public sealed record GetGuestsQuery(
        string? SearchTerm = null,
        SortDirection SortDirection = SortDirection.Ascending,
        string? SortBy = null,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<Result<PaginatedList<GuestDto>>>;
}