using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Reservations;
using HotelManagementSystem.Domain.Services;
using Microsoft.EntityFrameworkCore;
using MediatR;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Common.Errors;
using Microsoft.Extensions.Logging;
using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Entities.Rooms;
using HotelManagementSystem.Domain.ValueObjects.Money;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CreateReservations;

public sealed class CreateReservationsCommandHandler(
    IAppDbContext context,
    ICurrentUserService currentUser,
    ILogger<CreateReservationsCommandHandler> logger)
    : IRequestHandler<CreateReservationsCommand, Result<Guid>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly ILogger<CreateReservationsCommandHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(CreateReservationsCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<Guid>.Failure(ApplicationErrors.Authorization.Forbidden);

        if (request.CheckInDate >= request.CheckOutDate)
            return Result<Guid>.Failure(ReservationErrors.InvalidCheckInOutDates);

        Guest? guest;
        if (Roles.IsStaff(_currentUser.Roles) && request.GuestId is { } targetGuestId)
        {
            guest = await _context.Guests.FindAsync([targetGuestId], cancellationToken);
        }
        else
        {
            guest = await _context.Guests.FirstOrDefaultAsync(g => g.UserId == userId, cancellationToken);
        }

        if (guest is null)
        {
            _logger.LogWarning("Guest {GuestId} not found for reservation creation", _currentUser.UserId);
            return Result<Guid>.Failure(ApplicationErrors.GuestErrors.NotFound);
        }

        var roomCategory = await _context.RoomCategories.FindAsync([request.RoomCategoryId], cancellationToken);
        if (roomCategory is null)
        {
            _logger.LogWarning(
                "Room category {RoomCategoryId} not found for reservation creation",
                request.RoomCategoryId);
            return Result<Guid>.Failure(ApplicationErrors.RoomCategory.NotFound);
        }


        var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
        if (room is null)
        {
            _logger.LogWarning("Room {RoomId} not found for reservation creation", request.RoomId);
            return Result<Guid>.Failure(ApplicationErrors.Room.NotFound);
        }

        if (room.RoomCategoryId != roomCategory.Id || room.HotelId != roomCategory.HotelId || request.HotelId != roomCategory.HotelId)
        {
            _logger.LogWarning(
                "Room {RoomId} does not belong to category {RoomCategoryId} / hotel {HotelId}",
                request.RoomId, request.RoomCategoryId, request.HotelId);

            return Result<Guid>.Failure(ApplicationErrors.Room.NotFound);
        }

        if (room.Status == RoomStatus.OutOfOrder)
        {
            _logger.LogWarning("Room {RoomId} is out of service; cannot create reservation", request.RoomId);
            return Result<Guid>.Failure(RoomErrors.RoomNotAvailable);
        }

        var hasOverlap = await _context.Reservations
           .AsNoTracking()
           .AnyAsync(r =>
               r.RoomId == request.RoomId
               && (r.Status == ReservationStatus.Pending
                   || r.Status == ReservationStatus.Confirmed
                   || r.Status == ReservationStatus.CheckedIn)
               && request.CheckInDate < r.CheckOutDate
               && r.CheckInDate < request.CheckOutDate,
               cancellationToken);

        if (hasOverlap)
        {
            _logger.LogWarning(
                "Reservation overlap validation failed for guest {GuestId}, room {RoomId}",
                guest.Id, request.RoomId);
            return Result<Guid>.Failure(ReservationErrors.RoomAlreadyBooked);
        }

        var reservationNumber = $"R{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";

        var nights = request.CheckOutDate.DayNumber - request.CheckInDate.DayNumber;

        var totalAmountResult = roomCategory.BasePricePerNight.Multiply(nights);
        if (totalAmountResult.IsError)
        {
            _logger.LogWarning(
                "Failed to compute stay total for reservation ({Nights} nights, room {RoomId}): {Errors}",
                nights, request.RoomId, totalAmountResult.Errors);
            return Result<Guid>.Failure(totalAmountResult.Errors);
        }


        var createResult = Reservation.Create(
                   guestId: guest.Id,
                   hotelId: roomCategory.HotelId,
                   roomCategoryId: request.RoomCategoryId,
                   roomId: request.RoomId,
                   reservationNumber: reservationNumber,
                   checkInDate: request.CheckInDate,
                   checkOutDate: request.CheckOutDate,
                   numberOfAdults: request.NumberOfAdults,
                   numberOfChildren: request.NumberOfChildren,
                   basePricePerNight: roomCategory.BasePricePerNight,
                   totalAmount: totalAmountResult.Value,
                   specialRequests: request.SpecialRequests);

        if (createResult.IsError)
        {
            _logger.LogWarning("Failed to create reservation for guest {GuestId}, room {RoomId}: {Errors}", guest.Id, request.RoomId, createResult.Errors);
            return Result<Guid>.Failure(createResult.Errors);
        }

        await _context.Reservations.AddAsync(createResult.Value, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Reservation {ReservationId} created for guest {GuestId} in room {RoomId}", createResult.Value.Id, guest.Id, request.RoomId);
        return Result<Guid>.Success(createResult.Value.Id);
    }

}