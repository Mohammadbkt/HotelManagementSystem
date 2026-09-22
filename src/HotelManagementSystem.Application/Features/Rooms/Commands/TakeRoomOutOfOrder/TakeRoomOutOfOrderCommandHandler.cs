using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.TakeRoomOutOfOrder
{
    public sealed class TakeRoomOutOfOrderCommandHandler(IAppDbContext context, ILogger<TakeRoomOutOfOrderCommandHandler> logger) : IRequestHandler<TakeRoomOutOfOrderCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<TakeRoomOutOfOrderCommandHandler> _logger = logger;

        public async Task<Result> Handle(TakeRoomOutOfOrderCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if(room is null)
            {
                _logger.LogWarning("Room {RoomId} not found", request.RoomId);
                return Result.Failure(ApplicationErrors.Room.NotFound);
            }

            var roomResult = room.TakeOutOfOrder(request.Reason);
            if(roomResult.IsError)
            {
                _logger.LogWarning("Failed to take room {RoomId} out of order: {Errors}", request.RoomId, roomResult.Errors);
                return Result.Failure(roomResult.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Room {RoomId} marked out of order", request.RoomId);
            return Result.Success();
        }
    }
}