using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.TakeRoomOutOfOrder
{
    public sealed class TakeRoomOutOfOrderCommandHandler(IAppDbContext context) : IRequestHandler<TakeRoomOutOfOrderCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(TakeRoomOutOfOrderCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if(room is null)
                return Result.Failure(Error.NotFound("Room.NotFound", "Room not found"));

            var roomResult = room.TakeOutOfOrder(request.Reason);
            if(roomResult.IsError)
                return Result.Failure(roomResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}