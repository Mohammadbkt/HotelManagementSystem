using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.RoomCategories.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoriesByHotel
{
    public sealed record GetRoomCategoriesByHotelQuery(
        Guid HotelId,
        string? SearchTerm = null,
        int? MaxCapacity = null,
        decimal? MinPricePerNight = null,
        decimal? MaxPricePerNight = null,
        SortDirection SortDirection = SortDirection.Ascending,
        string? SortBy = null,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<Result<PaginatedList<RoomCategoryDto>>>;

}