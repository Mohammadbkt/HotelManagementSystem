using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Hotels.Dtos;
using HotelManagementSystem.Application.Features.Hotels.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Hotels.Queries.GetHotels
{
    public sealed class GetHotelsQueryHandler(IAppDbContext context) : IRequestHandler<GetHotelsQuery, Result<PaginatedList<HotelDto>>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<PaginatedList<HotelDto>>> Handle(GetHotelsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Hotels.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(h => h.Name.Contains(request.SearchTerm));

            if (!string.IsNullOrWhiteSpace(request.City))
                query = query.Where(h => h.Address.City.Contains(request.City));

            if (!string.IsNullOrWhiteSpace(request.Country))
                query = query.Where(h => h.Address.Country.Contains(request.Country));

            query = request.SortBy?.ToLowerInvariant() switch
            {
                "city" => request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(h => h.Address.City)
                    : query.OrderByDescending(h => h.Address.City),

                _ => request.SortDirection == SortDirection.Ascending
                   ? query.OrderBy(h => h.Name)
                   : query.OrderByDescending(h => h.Name)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToHotelDto()
                .ToListAsync(cancellationToken);

            var paginatedResult = new PaginatedList<HotelDto>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Items = items
            };

            return Result<PaginatedList<HotelDto>>.Success(paginatedResult);
        }
    }
}