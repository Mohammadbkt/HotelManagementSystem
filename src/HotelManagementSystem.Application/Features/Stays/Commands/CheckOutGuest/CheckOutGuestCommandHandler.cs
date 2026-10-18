using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Stays.Commands.CheckOutGuest
{
    public sealed class CheckOutGuestCommandHandler(IAppDbContext context, ILogger<CheckOutGuestCommandHandler> logger) : IRequestHandler<CheckOutGuestCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<CheckOutGuestCommandHandler> _logger = logger;

        public async Task<Result> Handle(CheckOutGuestCommand request, CancellationToken cancellationToken)
        {
            var stay = await _context.Stays.FindAsync([request.StayId], cancellationToken);
            if (stay is null)
            {
                _logger.LogWarning("Stay {StayId} not found", request.StayId);
                return Result.Failure(ApplicationErrors.Stay.NotFound);
            }

            var room = await _context.Rooms.FindAsync([stay.RoomId], cancellationToken);
            if (room is null)
            {
                _logger.LogWarning("Room {RoomId} not found while checking out stay {StayId}", stay.RoomId, request.StayId);
                return Result.Failure(ApplicationErrors.Room.NotFound);
            }

            if (stay.ReservationId.HasValue)
            {
                var reservation = await _context.Reservations.FindAsync([stay.ReservationId], cancellationToken);
                if (reservation is null)
                {
                    _logger.LogWarning("Reservation {ReservationId} not found while checking out stay {StayId}", stay.ReservationId, request.StayId);
                    return Result.Failure(ApplicationErrors.Reservation.NotFound);
                }

                var completeResult = reservation.MarkAsComplete();
                if (completeResult.IsError)
                {
                    _logger.LogWarning("Failed to mark reservation {ReservationId} complete during check-out of stay {StayId}: {Errors}", stay.ReservationId, request.StayId, completeResult.Errors);
                    return Result.Failure(completeResult.Errors);
                }
            }

            var stayResult = stay.CheckOut(DateTimeOffset.UtcNow);
            if (stayResult.IsError)
            {
                _logger.LogWarning("Failed to check out stay {StayId}: {Errors}", request.StayId, stayResult.Errors);
                return Result.Failure(stayResult.Errors);
            }

            var roomResult = room.CheckOut();
            if (roomResult.IsError)
            {
                _logger.LogWarning("Failed to check out room {RoomId} for stay {StayId}: {Errors}", stay.RoomId, request.StayId, roomResult.Errors);
                return Result.Failure(roomResult.Errors);
            }


            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Stay {StayId} checked out", request.StayId);
            return Result.Success();
        }
    }
}