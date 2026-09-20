using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.UpdateReservation
{
    public sealed class UpdateReservationCommandHandler(IAppDbContext context) : IRequestHandler<UpdateReservationCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(UpdateReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations.FindAsync([request.ReservationId], cancellationToken);
            if (reservation is null)
                return Result.Failure(Error.NotFound("Reservation.NotFound", "Reservation not found."));

            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
                return Result.Failure(Error.NotFound("Room.NotFound", "Room not found."));

            var existingReservationsForRoom = await _context.Reservations
                .Where(r => r.RoomId == request.RoomId && r.Id != request.ReservationId)
                .ToListAsync(cancellationToken);

            var availabilityResult = ReservationAvailabilityService.EnsureNoOverlap(
                existingReservationsForRoom, request.CheckInDate, request.CheckOutDate);

            if (availabilityResult.IsError)
                return Result.Failure(availabilityResult.Errors);

            var result = reservation.Update(request.RoomId, request.CheckInDate, request.CheckOutDate);
            if (result.IsError)
                return Result.Failure(result.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}