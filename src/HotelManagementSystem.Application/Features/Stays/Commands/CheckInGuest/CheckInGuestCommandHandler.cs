using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.Entities.Rooms;
using HotelManagementSystem.Domain.Entities.Stays;
using HotelManagementSystem.Domain.Services;
using Microsoft.EntityFrameworkCore;
using MediatR;
using HotelManagementSystem.Application.Common.Errors;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Stays.Commands.CheckInGuest
{
    public sealed class CheckInGuestCommandHandler(IAppDbContext context, ILogger<CheckInGuestCommandHandler> logger) : IRequestHandler<CheckInGuestCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<CheckInGuestCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(CheckInGuestCommand request, CancellationToken cancellationToken)
        {
            if (request.ReservationId is not null)
                return await CheckInWithReservation(request, cancellationToken);

            return await CheckInWalkIn(request, cancellationToken);
        }

        private async Task<Result<Guid>> CheckInWithReservation(CheckInGuestCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations.FindAsync([request.ReservationId], cancellationToken);
            if (reservation is null)
            {
                _logger.LogWarning("Reservation {ReservationId} not found during check-in", request.ReservationId);
                return Result<Guid>.Failure(ApplicationErrors.Reservation.NotFound);
            }

            var guest = await _context.Guests.FindAsync([reservation.GuestId], cancellationToken);
            if (guest is null)
            {
                _logger.LogWarning("Guest {GuestId} not found during check-in for reservation {ReservationId}", reservation.GuestId, request.ReservationId);
                return Result<Guid>.Failure(GuestErrors.NotFound);
            }

            var room = await _context.Rooms.FindAsync([reservation.RoomId], cancellationToken);
            if (room is null)
            {
                _logger.LogWarning("Room {RoomId} not found during check-in for reservation {ReservationId}", reservation.RoomId, request.ReservationId);
                return Result<Guid>.Failure(ApplicationErrors.Room.NotFound);
            }

            var stayResult = Stay.CreateFromReservation(reservation, DateTimeOffset.UtcNow);
            if (stayResult.IsError)
            {
                _logger.LogWarning("Failed to create stay from reservation {ReservationId}: {Errors}", request.ReservationId, stayResult.Errors);
                return Result<Guid>.Failure(stayResult.Errors);
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var reservationResult = reservation.MarkAsCheckedIn(today);
            if (reservationResult.IsError)
            {
                _logger.LogWarning("Failed to mark reservation {ReservationId} as checked in: {Errors}", request.ReservationId, reservationResult.Errors);
                return Result<Guid>.Failure(reservationResult.Errors);
            }

            var roomResult = room.CheckIn();
            if (roomResult.IsError)
            {
                _logger.LogWarning("Failed to check room {RoomId} in for reservation {ReservationId}: {Errors}", reservation.RoomId, request.ReservationId, roomResult.Errors);
                return Result<Guid>.Failure(roomResult.Errors);
            }

            await _context.Stays.AddAsync(stayResult.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Stay {StayId} created for reservation {ReservationId}", stayResult.Value.Id, request.ReservationId);
            return Result<Guid>.Success(stayResult.Value.Id);
        }

        private async Task<Result<Guid>> CheckInWalkIn(CheckInGuestCommand request, CancellationToken cancellationToken)
        {
            var guest = await _context.Guests.FindAsync([request.GuestId], cancellationToken);
            if (guest is null)
            {
                _logger.LogWarning("Guest {GuestId} not found during walk-in check-in", request.GuestId);
                return Result<Guid>.Failure(GuestErrors.NotFound);
            }

            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
            {
                _logger.LogWarning("Room {RoomId} not found during walk-in check-in", request.RoomId);
                return Result<Guid>.Failure(ApplicationErrors.Room.NotFound);
            }

            var stayResult = Stay.Create(request.GuestId, request.RoomId, DateTimeOffset.UtcNow);
            if (stayResult.IsError)
            {
                _logger.LogWarning("Failed to create walk-in stay for guest {GuestId}: {Errors}", request.GuestId, stayResult.Errors);
                return Result<Guid>.Failure(stayResult.Errors);
            }

            if (room.Status != RoomStatus.CleanVacant)
            {
                _logger.LogWarning("Room {RoomId} is not available for walk-in check-in", request.RoomId);
                return Result<Guid>.Failure(RoomErrors.RoomNotAvailable);
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var reservations = await _context.Reservations
                .Where(r => r.RoomId == request.RoomId && r.CheckOutDate > today)
                .ToListAsync(cancellationToken);

            var availability = ReservationAvailabilityService.EnsureNoOverlap(
                reservations, today, request.ExpectedCheckOutDate!.Value);
            if (availability.IsError)
            {
                _logger.LogWarning("Walk-in check-in for guest {GuestId} room {RoomId} failed availability check: {Errors}", request.GuestId, request.RoomId, availability.Errors);
                return Result<Guid>.Failure(availability.Errors);
            }

            var roomResult = room.CheckIn();
            if (roomResult.IsError)
            {
                _logger.LogWarning("Failed to check room {RoomId} in for guest {GuestId}: {Errors}", request.RoomId, request.GuestId, roomResult.Errors);
                return Result<Guid>.Failure(roomResult.Errors);
            }

            await _context.Stays.AddAsync(stayResult.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Walk-in stay {StayId} created for guest {GuestId} in room {RoomId}", stayResult.Value.Id, request.GuestId, request.RoomId);
            return Result<Guid>.Success(stayResult.Value.Id);
        }
    }
}