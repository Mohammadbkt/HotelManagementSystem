using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.MarkRoomAsClean
{
    public sealed class MarkRoomAsCleanCommandHandler(IAppDbContext context) : IRequestHandler<MarkRoomAsCleanCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(MarkRoomAsCleanCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
                return Result.Failure(Error.NotFound("Room.NotFound", "Room not found"));

            var roomResult = room.MarkAsClean();
            if (roomResult.IsError)
                return Result.Failure(roomResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}