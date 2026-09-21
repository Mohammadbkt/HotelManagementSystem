using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Reservations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomCategory
{
    public sealed class ChangeRoomCategoryCommandHandler(IAppDbContext context) : IRequestHandler<ChangeRoomCategoryCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(ChangeRoomCategoryCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
                return Result.Failure(ApplicationErrors.Room.NotFound);

            var roomCategory = await _context.RoomCategories.FindAsync([request.RoomCategoryId], cancellationToken);
            if (roomCategory is null)
                return Result.Failure(ApplicationErrors.RoomCategory.NotFound);

            if (roomCategory.HotelId != room.HotelId)
                return Result.Failure(Error.Validation("Room.CategoryHotelMismatch", "The selected room category does not belong to this hotel."));

            var hasActiveReservation = await _context.Reservations
                                        .AnyAsync(r => r.RoomId == request.RoomId
                                                && (
                                                    r.Status == ReservationStatus.Confirmed
                                                    || r.Status == ReservationStatus.CheckedIn
                                                    || r.Status == ReservationStatus.Pending
                                                ), cancellationToken);

            if (hasActiveReservation)
                return Result.Failure(Error.Validation("Room.HasActiveReservation",
                    "Cannot change category: the room has an active reservation."));

            var hasActiveStay = await _context.Stays.AnyAsync(
                s => s.RoomId == request.RoomId 
                  && s.CheckOutDate == null,
                cancellationToken
            );

            if (hasActiveStay)
                return Result.Failure(Error.Validation("Room.HasActiveStay",
                    "Cannot change category: the room has an active stay."));

            var categoryResult = room.ChangeCategory(request.RoomCategoryId);
            if (categoryResult.IsError)
                return Result.Failure(categoryResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}