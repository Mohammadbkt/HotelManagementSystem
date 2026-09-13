using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomCategory
{
    public sealed class ChangeRoomCategoryCommandHandler(IAppDbContext context) : IRequestHandler<ChangeRoomCategoryCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(ChangeRoomCategoryCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FindAsync(request.RoomId, cancellationToken);
            if (room is null)
                return Result.Failure(Error.NotFound("Room.NotFound", "Room not found."));

            var roomCategory = await _context.RoomCategories.FindAsync(request.RoomCategoryId, cancellationToken);
            if (roomCategory is null)
                return Result.Failure(Error.NotFound("RoomCategory.NotFound", "Room category not found."));

            if (roomCategory.HotelId != room.HotelId)
                return Result.Failure(Error.Validation("Room.CategoryHotelMismatch", "The selected room category does not belong to this hotel."));


            //TODO:A room category cannot be changed while the room has an active stay or reservation that would be affected.

            var categoryResult = room.ChangeCategory(request.RoomCategoryId);
            if (categoryResult.IsError)
                return Result.Failure(categoryResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}