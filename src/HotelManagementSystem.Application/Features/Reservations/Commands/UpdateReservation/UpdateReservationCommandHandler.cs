using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Rooms;
using HotelManagementSystem.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.UpdateReservation
{
    public sealed class UpdateReservationCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateReservationCommandHandler> logger
        )
        : IRequestHandler<UpdateReservationCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ICurrentUserService _currentUser = currentUser;
        private readonly ILogger<UpdateReservationCommandHandler> _logger = logger;


        public async Task<Result> Handle(UpdateReservationCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                return Result.Failure(ApplicationErrors.Authorization.Forbidden);


            var userId = _currentUser.UserId.Value;

            var query = _context.Reservations.Where(r => r.Id == request.ReservationId);

            if (!Roles.IsStaff(_currentUser.Roles))
            {
                query = query.Where(r => _context.Guests.Any(g => g.Id == r.GuestId && g.UserId == userId));
            }

            var reservation = await query.FirstOrDefaultAsync(cancellationToken);
            if (reservation is null)
            {
                _logger.LogInformation("");
                return Result.Failure(ApplicationErrors.Reservation.NotFound);
            }

            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
            {
                _logger.LogInformation("");
                return Result.Failure(ApplicationErrors.Room.NotFound);
            }
            var overlapping = await _context.Reservations
                   .AsNoTracking()
                   .Where(r => r.RoomId == request.RoomId
                               && r.Id != request.ReservationId
                               && request.CheckInDate < r.CheckOutDate
                               && r.CheckInDate < request.CheckOutDate)
                   .ToListAsync(cancellationToken);


            var availabilityResult = ReservationAvailabilityService.EnsureNoOverlap(
                overlapping, request.CheckInDate, request.CheckOutDate);

            if (availabilityResult.IsError)
            {
                _logger.LogWarning("Update of reservation {ReservationId} rejected: {Errors}",
                                    request.ReservationId, availabilityResult.Errors);
                return Result.Failure(availabilityResult.Errors);
            }

            var result = reservation.Update(request.RoomId, request.CheckInDate, request.CheckOutDate);
            if (result.IsError)
            {
                _logger.LogWarning("Failed to update reservation {ReservationId}: {Errors}",
                                    request.ReservationId, result.Errors);
                return Result.Failure(result.Errors);
            }
            await _context.SaveChangesAsync(cancellationToken);


            _logger.LogInformation("Reservation {ReservationId} updated by {UserId}", request.ReservationId, userId);
            return Result.Success();
        }
    }
}