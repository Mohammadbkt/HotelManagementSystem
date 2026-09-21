using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
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
            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
                return Result.Failure(ApplicationErrors.Room.NotFound);

            var duplicateExists  = await _context.Rooms
                .AnyAsync(r =>  r.Id != room.Id 
                            && r.RoomNumber == request.RoomNumber
                            && r.HotelId == room.HotelId, cancellationToken);

            if (duplicateExists )
                return Result.Failure(ApplicationErrors.Room.AlreadyExists);

            var roomNumberResult = room.ChangeRoomNumber(request.RoomNumber);
            if (roomNumberResult.IsError)
                return Result.Failure(roomNumberResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}