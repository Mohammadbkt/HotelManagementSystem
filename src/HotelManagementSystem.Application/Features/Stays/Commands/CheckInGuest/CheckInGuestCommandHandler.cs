using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.Entities.Rooms;
using HotelManagementSystem.Domain.Entities.Stays;
using HotelManagementSystem.Domain.Services;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace HotelManagementSystem.Application.Features.Stays.Commands.CheckInGuest
{
    public sealed class CheckInGuestCommandHandler(IAppDbContext context) : IRequestHandler<CheckInGuestCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;

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
                return Result<Guid>.Failure(Error.NotFound("Reservation.NotFound", "Reservation not found."));

            var guest = await _context.Guests.FindAsync([reservation.GuestId], cancellationToken);
            if (guest is null)
                return Result<Guid>.Failure(GuestErrors.NotFound);

            var room = await _context.Rooms.FindAsync([reservation.RoomId], cancellationToken);
            if (room is null)
                return Result<Guid>.Failure(Error.NotFound("Room.NotFound", "Room not found."));

            var stayResult = Stay.CreateFromReservation(reservation, DateTimeOffset.UtcNow);
            if (stayResult.IsError)
                return Result<Guid>.Failure(stayResult.Errors);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var reservationResult = reservation.MarkAsCheckedIn(today);
            if (reservationResult.IsError)
                return Result<Guid>.Failure(reservationResult.Errors);

            var roomResult = room.CheckIn();
            if (roomResult.IsError)
                return Result<Guid>.Failure(roomResult.Errors);

            await _context.Stays.AddAsync(stayResult.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(stayResult.Value.Id);
        }

        private async Task<Result<Guid>> CheckInWalkIn(CheckInGuestCommand request, CancellationToken cancellationToken)
        {
            var guest = await _context.Guests.FindAsync([request.GuestId], cancellationToken);
            if (guest is null)
                return Result<Guid>.Failure(GuestErrors.NotFound);

            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
                return Result<Guid>.Failure(Error.NotFound("Room.NotFound", "Room not found."));

            var stayResult = Stay.Create(request.GuestId, request.RoomId, DateTimeOffset.UtcNow);
            if (stayResult.IsError)
                return Result<Guid>.Failure(stayResult.Errors);

            if (room.Status != RoomStatus.CleanVacant)
                return Result<Guid>.Failure(RoomErrors.RoomNotAvailable);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var reservations = await _context.Reservations
                .Where(r => r.RoomId == request.RoomId && r.CheckOutDate > today)
                .ToListAsync(cancellationToken);

            var availability = ReservationAvailabilityService.EnsureNoOverlap(
                reservations, today, request.ExpectedCheckOutDate!.Value);
            if (availability.IsError)
                return Result<Guid>.Failure(availability.Errors);

            var roomResult = room.CheckIn();
            if (roomResult.IsError)
                return Result<Guid>.Failure(roomResult.Errors);

            await _context.Stays.AddAsync(stayResult.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(stayResult.Value.Id);
        }
    }
}