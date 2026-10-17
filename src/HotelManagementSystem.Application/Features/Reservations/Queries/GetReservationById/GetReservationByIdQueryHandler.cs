using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Reservations.Dtos;
using HotelManagementSystem.Application.Features.Reservations.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Reservations.Queries.GetReservationById
{
    public sealed class GetReservationByIdQueryHandler(
    IAppDbContext context,
    ICurrentUserService currentUser,
    ILogger<GetReservationByIdQueryHandler> logger)
    : IRequestHandler<GetReservationByIdQuery, Result<ReservationDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ICurrentUserService _currentUser = currentUser;
        private readonly ILogger<GetReservationByIdQueryHandler> _logger = logger;

        public async Task<Result<ReservationDto>> Handle(
            GetReservationByIdQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
                return Result<ReservationDto>.Failure(ApplicationErrors.Reservation.NotFound);

            var query = _context.Reservations
                .AsNoTracking()
                .Where(r => r.Id == request.ReservationId);


            if (!Roles.IsStaff(_currentUser.Roles))
            {
                query = query.Where(r => _context.Guests.Any(g => g.Id == r.GuestId && g.UserId == userId));
            }


            var dto = await query
                    .ToReservationDto()
                    .FirstOrDefaultAsync(cancellationToken);

            if (dto is null)
            {
                _logger.LogWarning(
                    "Reservation {ReservationId} not found or not accessible to user {UserId}",
                    request.ReservationId, userId);
                return Result<ReservationDto>.Failure(ApplicationErrors.Reservation.NotFound);
            }

            return Result<ReservationDto>.Success(dto);
        }
    }
}