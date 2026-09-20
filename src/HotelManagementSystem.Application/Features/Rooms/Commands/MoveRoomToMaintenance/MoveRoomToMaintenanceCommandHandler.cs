using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.MoveRoomToMaintenance
{
    public sealed class MoveRoomToMaintenanceCommandHandler(IAppDbContext context) : IRequestHandler<MoveRoomToMaintenanceCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(MoveRoomToMaintenanceCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
                return Result.Failure(Error.NotFound("Room.NotFound", "Room not found"));

            var roomResult = room.MoveToMaintenance(request.Reason);
            if (roomResult.IsError)
                return Result.Failure(roomResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}