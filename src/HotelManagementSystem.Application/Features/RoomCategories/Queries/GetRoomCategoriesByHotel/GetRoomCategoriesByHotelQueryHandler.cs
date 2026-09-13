using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.RoomCategories.Dtos;
using HotelManagementSystem.Application.Features.RoomCategories.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoriesByHotel
{
    public class GetRoomCategoriesByHotelQueryHandler(IAppDbContext context) : IRequestHandler<GetRoomCategoriesByHotelQuery, Result<PaginatedList<RoomCategoryDto>>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<PaginatedList<RoomCategoryDto>>> Handle(GetRoomCategoriesByHotelQuery request, CancellationToken cancellationToken)
        {
            var query = _context.RoomCategories.AsNoTracking().AsQueryable()
                                    .Where(x => x.HotelId == request.HotelId);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x =>
                    x.Name.Contains(request.SearchTerm));
            }

            if (request.MaxCapacity.HasValue)
            {
                query = query.Where(x =>
                    x.MaxCapacity <= request.MaxCapacity.Value);
            }

            if (request.MinPricePerNight.HasValue)
            {
                query = query.Where(x =>
                    x.BasePricePerNight.Amount >= request.MinPricePerNight.Value);
            }

            if (request.MaxPricePerNight.HasValue)
            {
                query = query.Where(x =>
                    x.BasePricePerNight.Amount <= request.MaxPricePerNight.Value);
            }

            query = request.SortBy?.ToLowerInvariant() switch
            {

                "maxcapacity" =>
                    request.SortDirection == SortDirection.Ascending
                        ? query.OrderBy(x => x.MaxCapacity)
                        : query.OrderByDescending(x => x.MaxCapacity),

                "price" =>
                    request.SortDirection == SortDirection.Ascending
                        ? query.OrderBy(x => x.BasePricePerNight.Amount)
                        : query.OrderByDescending(x => x.BasePricePerNight.Amount),

                _ => request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(rc => rc.Name)
                    : query.OrderByDescending(rc => rc.Name)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToRoomCategoryDto()
            .ToListAsync(cancellationToken);

            var pagiantedResult = new PaginatedList<RoomCategoryDto>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Items = items
            };

            return Result<PaginatedList<RoomCategoryDto>>.Success(pagiantedResult);
        }
    }
}