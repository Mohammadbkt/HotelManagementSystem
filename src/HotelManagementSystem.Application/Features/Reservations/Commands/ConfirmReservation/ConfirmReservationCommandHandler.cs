using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.ConfirmReservation
{
    public sealed class ConfirmReservationCommandHandler(
    IAppDbContext context,
    ICurrentUserService currentUser,
    ILogger<ConfirmReservationCommandHandler> logger)
    : IRequestHandler<ConfirmReservationCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ICurrentUserService _currentUser = currentUser;
        private readonly ILogger<ConfirmReservationCommandHandler> _logger = logger;

        public async Task<Result> Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null || !Roles.IsStaff(_currentUser.Roles))
            {
                _logger.LogWarning("Confirm of reservation {ReservationId} denied for user {UserId}",
                    request.ReservationId, _currentUser.UserId);
                return Result.Failure(ApplicationErrors.Authorization.Forbidden);
            }

            var userId = _currentUser.UserId;

            var reservation = await _context.Reservations.FindAsync([request.ReservationId], cancellationToken);
            if (reservation is null)
            {
                _logger.LogWarning("Reservation {ReservationId} not found", request.ReservationId);
                return Result.Failure(ApplicationErrors.Reservation.NotFound);
            }

            var result = reservation.Confirm();
            if (result.IsError)
            {
                _logger.LogWarning("Failed to confirm reservation {ReservationId}: {Errors}",
                    request.ReservationId, result.Errors);
                return Result.Failure(result.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reservation {ReservationId} confirmed by {UserId}",
                request.ReservationId, userId);
            return Result.Success();
        }
    }
}