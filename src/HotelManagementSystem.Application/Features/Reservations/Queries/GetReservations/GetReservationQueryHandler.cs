using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Reservations.Dtos;
using HotelManagementSystem.Application.Features.Reservations.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Reservations.Queries.GetReservations
{
    public sealed class GetReservationsQueryHandler(IAppDbContext context) : IRequestHandler<GetReservationsQuery, Result<PaginatedList<ReservationDto>>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<PaginatedList<ReservationDto>>> Handle(GetReservationsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Reservations
                            .AsNoTracking()
                            .AsQueryable();

            if (request.GuestId.HasValue)
                query = query.Where(r => r.Id == request.GuestId);

            if (request.RoomId.HasValue)
                query = query.Where(r => r.RoomId == request.RoomId);

            if (request.Status.HasValue)
                query = query.Where(r => r.Status == request.Status);

            if (request.CheckInDate.HasValue)
                query = query.Where(r => r.CheckInDate == request.CheckInDate);

            if (request.CheckOutDate.HasValue)
                query = query.Where(r => r.CheckOutDate == request.CheckOutDate);

            query = request.SortBy?.ToLowerInvariant() switch
            {
                "checkoutdate" => request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(r => r.CheckOutDate)
                    : query.OrderByDescending(r => r.CheckOutDate),

                "status" => request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(r => r.Status)
                    : query.OrderByDescending(r => r.Status),

                _ => request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(r => r.CheckInDate)
                    : query.OrderByDescending(r => r.CheckInDate)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToReservationDto()
                        .ToListAsync(cancellationToken);

            var paginatedResult = new PaginatedList<ReservationDto>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Items = items
            };

            return Result<PaginatedList<ReservationDto>>.Success(paginatedResult);


        }
    }
}