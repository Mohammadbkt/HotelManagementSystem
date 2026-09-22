using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Stays.Dtos;
using HotelManagementSystem.Application.Features.Stays.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Stays.Queries.GetStays
{
    public sealed class GetStaysQueryHandler(IAppDbContext context, ILogger<GetStaysQueryHandler> logger) : IRequestHandler<GetStaysQuery, Result<PaginatedList<StayDto>>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetStaysQueryHandler> _logger = logger;

        public async Task<Result<PaginatedList<StayDto>>> Handle(GetStaysQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Stays
                            .AsNoTracking()
                            .AsQueryable();

            if (request.GuestId.HasValue)
                query = query.Where(s => s.GuestId == request.GuestId);


            if (request.RoomId.HasValue)
                query = query.Where(s => s.RoomId == request.RoomId);

            if (request.ReservationId.HasValue)
                query = query.Where(s => s.ReservationId == request.ReservationId);

            if (request.IsActive.HasValue)
                query = request.IsActive.Value
                ? query.Where(s => s.CheckOutDate == null)
                : query.Where(s => s.CheckOutDate != null);

            if (request.CheckInFrom.HasValue)
                query = query.Where(s => s.CheckInDate >= request.CheckInFrom);

            if (request.CheckOutFrom.HasValue)
                query = query.Where(s => s.CheckOutDate >= request.CheckOutFrom);

            if (request.CheckInTo.HasValue)
                query = query.Where(s => s.CheckInDate <= request.CheckInTo);

            if (request.CheckOutTo.HasValue)
                query = query.Where(s => s.CheckOutDate <= request.CheckOutTo);

            query = request.SortBy?.ToLowerInvariant() switch
            {
                "checkindate" => request.SortDirection == SortDirection.Ascending
                                ? query.OrderBy(s => s.CheckInDate)
                                : query.OrderByDescending(s => s.CheckInDate),


                "checkoutdate" => request.SortDirection == SortDirection.Ascending
                                ? query.OrderBy(s => s.CheckOutDate)
                                : query.OrderByDescending(s => s.CheckOutDate),

                _ => request.SortDirection == SortDirection.Ascending
                                ? query.OrderBy(s => s.CheckInDate)
                                : query.OrderByDescending(s => s.CheckInDate)

            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToStayDto()
                        .ToListAsync(cancellationToken);

            var paginatedResult = new PaginatedList<StayDto>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Items = items
            };

            _logger.LogInformation("Retrieved {TotalCount} stays (page {PageNumber}/{TotalPages})", totalCount, request.PageNumber, paginatedResult.TotalPages);
            return Result<PaginatedList<StayDto>>.Success(paginatedResult);



        }
    }
}