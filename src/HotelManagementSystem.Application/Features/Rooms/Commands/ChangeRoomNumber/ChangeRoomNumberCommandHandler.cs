using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomNumber;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomNumber
{
    public sealed class ChangeRoomNumberCommandHandler(IAppDbContext context) : IRequestHandler<ChangeRoomNumberCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(ChangeRoomNumberCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FindAsync(request.RoomId, cancellationToken);
            if (room is null)
                return Result.Failure(Error.NotFound("Room.NotFound", "Room not found."));

            var duplicateExists  = await _context.Rooms
                .AnyAsync(r =>  r.Id != room.Id 
                            && r.RoomNumber == request.RoomNumber
                            && r.HotelId == room.HotelId, cancellationToken);

            if (duplicateExists )
                return Result.Failure(
                    Error.Conflict(
                        "Room.RoomNumberAlreadyExists",
                        "A room with this number already exists in this hotel."));

            //TODO:A room number cannot be changed while the room has an active stay or reservation that would be affected.

            var roomNumberResult = room.ChangeRoomNumber(request.RoomNumber);
            if (roomNumberResult.IsError)
                return Result.Failure(roomNumberResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}