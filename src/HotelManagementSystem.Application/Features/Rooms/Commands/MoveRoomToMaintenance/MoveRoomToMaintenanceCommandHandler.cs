using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.MoveRoomToMaintenance
{
    public sealed class MoveRoomToMaintenanceCommandHandler(IAppDbContext context, ILogger<MoveRoomToMaintenanceCommandHandler> logger) : IRequestHandler<MoveRoomToMaintenanceCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<MoveRoomToMaintenanceCommandHandler> _logger = logger;

        public async Task<Result> Handle(MoveRoomToMaintenanceCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
            {
                _logger.LogWarning("Room {RoomId} not found", request.RoomId);
                return Result.Failure(ApplicationErrors.Room.NotFound);
            }

            var roomResult = room.MoveToMaintenance(request.Reason);
            if (roomResult.IsError)
            {
                _logger.LogWarning("Failed to move room {RoomId} to maintenance: {Errors}", request.RoomId, roomResult.Errors);
                return Result.Failure(roomResult.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Room {RoomId} moved to maintenance", request.RoomId);
            return Result.Success();

        }
    }
}