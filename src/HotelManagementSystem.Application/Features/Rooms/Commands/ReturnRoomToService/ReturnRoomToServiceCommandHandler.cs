using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ReturnRoomToService
{
    public sealed class ReturnRoomToServiceCommandHandler(IAppDbContext context, ILogger<ReturnRoomToServiceCommandHandler> logger) : IRequestHandler<ReturnRoomToServiceCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<ReturnRoomToServiceCommandHandler> _logger = logger;

        public async Task<Result> Handle(ReturnRoomToServiceCommand request, CancellationToken cancellationToken)
        {
             var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
            {
                _logger.LogWarning("Room {RoomId} not found", request.RoomId);
                return Result.Failure(ApplicationErrors.Room.NotFound);
            }

            var roomResult = room.ReturnToService();
            if (roomResult.IsError)
            {
                _logger.LogWarning("Failed to return room {RoomId} to service: {Errors}", request.RoomId, roomResult.Errors);
                return Result.Failure(roomResult.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Room {RoomId} returned to service", request.RoomId);
            return Result.Success();
        }
    }
}