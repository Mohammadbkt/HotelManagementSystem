using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Reservations;
using HotelManagementSystem.Domain.Services;
using Microsoft.EntityFrameworkCore;
using MediatR;
using HotelManagementSystem.Application.Common.Interfaces;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CreateReservations;

public sealed class CreateReservationsCommandHandler(IAppDbContext context) : IRequestHandler<CreateReservationsCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Guid>> Handle(CreateReservationsCommand request, CancellationToken cancellationToken)
    {
        var guest = await _context.Guests.FindAsync([request.GuestId], cancellationToken);

        if (guest is null)
            return Result<Guid>.Failure(Error.NotFound("Guest.NotFound", "Guest not found."));

        var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
        if (room is null)
            return Result<Guid>.Failure(Error.NotFound("Room.NotFound", "Room not found."));


        var existingReservationsForRoom = await _context.Reservations
            .Where(r => r.RoomId == request.RoomId)
            .ToListAsync(cancellationToken);

        var availabilityResult = ReservationAvailabilityService.EnsureNoOverlap(
            existingReservationsForRoom, request.CheckInDate, request.CheckOutDate);

        if (availabilityResult.IsError)
            return Result<Guid>.Failure(availabilityResult.Errors);



        var result = Reservation.Create(request.GuestId, request.RoomId, request.CheckInDate, request.CheckOutDate);

        if (result.IsError)
            return Result<Guid>.Failure(result.Errors);

        await _context.Reservations.AddAsync(result.Value, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(result.Value.Id);
    }
}