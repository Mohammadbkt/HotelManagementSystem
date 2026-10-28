using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CancelReservation
{
    public sealed class CancelReservationCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<CancelReservationCommandHandler> logger)
        : IRequestHandler<CancelReservationCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ICurrentUserService _currentUser = currentUser;
        private readonly ILogger<CancelReservationCommandHandler> _logger = logger;

        public async Task<Result> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
                return Result.Failure(ApplicationErrors.Reservation.NotFound);

            var query = _context.Reservations.Where(r => r.Id == request.ReservationId);

            if (!Roles.IsStaff(_currentUser.Roles))
            {
                query = query.Where(r => _context.Guests.Any(g => g.Id == r.GuestId && g.UserId == userId));
            }

            var reservation = await query.FirstOrDefaultAsync(cancellationToken);
            if (reservation is null)
            {
                _logger.LogWarning(
                    "Reservation {ReservationId} not found or not accessible to user {UserId}",
                    request.ReservationId, userId);
                return Result.Failure(ApplicationErrors.Reservation.NotFound);
            }

            var result = reservation.Cancel(DateTimeOffset.UtcNow, request.Reason);
            if (result.IsError)
            {
                _logger.LogWarning("Failed to cancel reservation {ReservationId}: {Errors}",
                    request.ReservationId, result.Errors);
                return Result.Failure(result.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reservation {ReservationId} cancelled by {UserId}",
                request.ReservationId, userId);
            return Result.Success();
        }
    }
}