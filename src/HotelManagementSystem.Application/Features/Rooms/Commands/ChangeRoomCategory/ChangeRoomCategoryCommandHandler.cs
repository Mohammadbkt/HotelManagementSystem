using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Reservations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomCategory
{
    public sealed class ChangeRoomCategoryCommandHandler(IAppDbContext context, ILogger<ChangeRoomCategoryCommandHandler> logger) : IRequestHandler<ChangeRoomCategoryCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<ChangeRoomCategoryCommandHandler> _logger = logger;

        public async Task<Result> Handle(ChangeRoomCategoryCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
            {
                _logger.LogWarning("Room {RoomId} not found", request.RoomId);
                return Result.Failure(ApplicationErrors.Room.NotFound);
            }

            var roomCategory = await _context.RoomCategories.FindAsync([request.RoomCategoryId], cancellationToken);
            if (roomCategory is null)
            {
                _logger.LogWarning("Room category {RoomCategoryId} not found", request.RoomCategoryId);
                return Result.Failure(ApplicationErrors.RoomCategory.NotFound);
            }

            if (roomCategory.HotelId != room.HotelId)
            {
                _logger.LogWarning("Room category {RoomCategoryId} is not valid for hotel {HotelId}", request.RoomCategoryId, room.HotelId);
                return Result.Failure(ApplicationErrors.RoomCategory.HotelMismatch);
            }

            var hasActiveReservation = await _context.Reservations
                                        .AnyAsync(r => r.RoomId == request.RoomId
                                                && (
                                                    r.Status == ReservationStatus.Confirmed
                                                    || r.Status == ReservationStatus.CheckedIn
                                                    || r.Status == ReservationStatus.Pending
                                                ), cancellationToken);

            if (hasActiveReservation)
            {
                _logger.LogWarning("Room {RoomId} has an active reservation and cannot change category", request.RoomId);
                return Result.Failure(ApplicationErrors.Room.HasActiveReservation);
            }

            var hasActiveStay = await _context.Stays.AnyAsync(
                s => s.RoomId == request.RoomId 
                  && s.CheckOutDate == null,
                cancellationToken
            );

            if (hasActiveStay)
            {
                _logger.LogWarning("Room {RoomId} has an active stay and cannot change category", request.RoomId);
                return Result.Failure(ApplicationErrors.Room.HasActiveStay);
            }

            var categoryResult = room.ChangeCategory(request.RoomCategoryId);
            if (categoryResult.IsError)
            {
                _logger.LogWarning("Failed to change room category for room {RoomId}: {Errors}", request.RoomId, categoryResult.Errors);
                return Result.Failure(categoryResult.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Room {RoomId} category changed to {RoomCategoryId}", request.RoomId, request.RoomCategoryId);
            return Result.Success();

        }
    }
}