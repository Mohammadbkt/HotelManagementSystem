using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomNumber;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomNumber
{
    public sealed class ChangeRoomNumberCommandHandler(IAppDbContext context, ILogger<ChangeRoomNumberCommandHandler> logger) : IRequestHandler<ChangeRoomNumberCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<ChangeRoomNumberCommandHandler> _logger = logger;

        public async Task<Result> Handle(ChangeRoomNumberCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
            {
                _logger.LogWarning("Room {RoomId} not found", request.RoomId);
                return Result.Failure(ApplicationErrors.Room.NotFound);
            }

            var duplicateExists  = await _context.Rooms
                .AnyAsync(r =>  r.Id != room.Id 
                            && r.RoomNumber == request.RoomNumber
                            && r.HotelId == room.HotelId, cancellationToken);

            if (duplicateExists )
            {
                _logger.LogWarning("Room number {RoomNumber} already exists for hotel {HotelId}", request.RoomNumber, room.HotelId);
                return Result.Failure(ApplicationErrors.Room.AlreadyExists);
            }

            var roomNumberResult = room.ChangeRoomNumber(request.RoomNumber);
            if (roomNumberResult.IsError)
            {
                _logger.LogWarning("Failed to change room number for room {RoomId}: {Errors}", request.RoomId, roomNumberResult.Errors);
                return Result.Failure(roomNumberResult.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Room {RoomId} room number changed to {RoomNumber}", request.RoomId, request.RoomNumber);
            return Result.Success();

        }
    }
}