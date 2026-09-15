using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Rooms.Dtos;
using HotelManagementSystem.Application.Features.Rooms.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Rooms.Queries.GetRooms
{
    public sealed class GetRoomsQueryHandler(IAppDbContext context) : IRequestHandler<GetRoomsQuery, Result<PaginatedList<RoomDto>>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<PaginatedList<RoomDto>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Rooms
                        .AsNoTracking()
                        .AsQueryable()
                        .Where(r => r.HotelId == request.HotelId);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(r => r.RoomNumber.Contains(request.SearchTerm));

            if (request.Status.HasValue)
                query = query.Where(r => r.Status == request.Status);

            query = request.SortBy?.ToLowerInvariant() switch
            {
                "roomnumber" => request.SortDirection == SortDirection.Ascending
                ? query.OrderBy(r => r.RoomNumber)
                : query.OrderByDescending(r => r.RoomNumber),

                _ => request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(r => r.RoomNumber)
                    : query.OrderByDescending(r => r.RoomNumber)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToRoomDto()
                        .ToListAsync(cancellationToken);

            var paginatedResult = new PaginatedList<RoomDto>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Items = items
            };

             return Result<PaginatedList<RoomDto>>.Success(paginatedResult);

        }
    }
}