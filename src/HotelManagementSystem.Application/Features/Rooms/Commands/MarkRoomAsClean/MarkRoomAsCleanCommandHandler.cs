using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.MarkRoomAsClean
{
    public sealed class MarkRoomAsCleanCommandHandler(IAppDbContext context, ILogger<MarkRoomAsCleanCommandHandler> logger) : IRequestHandler<MarkRoomAsCleanCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<MarkRoomAsCleanCommandHandler> _logger = logger;

        public async Task<Result> Handle(MarkRoomAsCleanCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
            {
                _logger.LogWarning("Room {RoomId} not found", request.RoomId);
                return Result.Failure(ApplicationErrors.Room.NotFound);
            }

            var roomResult = room.MarkAsClean();
            if (roomResult.IsError)
            {
                _logger.LogWarning("Failed to mark room {RoomId} as clean: {Errors}", request.RoomId, roomResult.Errors);
                return Result.Failure(roomResult.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Room {RoomId} marked as clean", request.RoomId);
            return Result.Success();

        }
    }
}