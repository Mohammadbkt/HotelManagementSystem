using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Hotels.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Hotels.Queries.GetHotels
{
    public sealed record GetHotelsQuery(
            string? SearchTerm = null,
            string? City = null,
            string? Country = null,
            string? SortBy = null,
            SortDirection SortDirection = SortDirection.Ascending,
            int PageNumber = 1,
            int PageSize = 10
    ) : IRequest<Result<PaginatedList<HotelDto>>>;
} 