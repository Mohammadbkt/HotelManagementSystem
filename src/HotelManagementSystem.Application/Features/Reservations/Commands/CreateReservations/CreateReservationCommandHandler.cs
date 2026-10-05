using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Reservations;
using HotelManagementSystem.Domain.Services;
using Microsoft.EntityFrameworkCore;
using MediatR;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Common.Errors;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CreateReservations;

public sealed class CreateReservationsCommandHandler(IAppDbContext context, ILogger<CreateReservationsCommandHandler> logger) : IRequestHandler<CreateReservationsCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreateReservationsCommandHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(CreateReservationsCommand request, CancellationToken cancellationToken)
    {
        var guest = await _context.Guests.FindAsync([request.GuestId], cancellationToken);

        if (guest is null)
        {
            _logger.LogWarning("Guest {GuestId} not found for reservation creation", request.GuestId);
            return Result<Guid>.Failure(ApplicationErrors.GuestErrors.NotFound);
        }

        var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
        if (room is null)
        {
            _logger.LogWarning("Room {RoomId} not found for reservation creation", request.RoomId);
            return Result<Guid>.Failure(ApplicationErrors.Room.NotFound);
        }


        var existingReservationsForRoom = await _context.Reservations
            .Where(r => r.RoomId == request.RoomId)
            .ToListAsync(cancellationToken);

        var availabilityResult = ReservationAvailabilityService.EnsureNoOverlap(
            existingReservationsForRoom, request.CheckInDate, request.CheckOutDate);

        if (availabilityResult.IsError)
        {
            _logger.LogWarning("Reservation overlap validation failed for guest {GuestId}, room {RoomId}: {Errors}", request.GuestId, request.RoomId, availabilityResult.Errors);
            return Result<Guid>.Failure(availabilityResult.Errors);
        }



        var result = Reservation.Create(request.GuestId, request.RoomId, request.CheckInDate, request.CheckOutDate);

        if (result.IsError)
        {
            _logger.LogWarning("Failed to create reservation for guest {GuestId}, room {RoomId}: {Errors}", request.GuestId, request.RoomId, result.Errors);
            return Result<Guid>.Failure(result.Errors);
        }

        await _context.Reservations.AddAsync(result.Value, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Reservation {ReservationId} created for guest {GuestId} in room {RoomId}", result.Value.Id, request.GuestId, request.RoomId);
        return Result<Guid>.Success(result.Value.Id);
    }
}