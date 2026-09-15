using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Stays.Commands.CheckOutGuest
{
    public sealed class CheckOutGuestCommandHandler(IAppDbContext context) : IRequestHandler<CheckOutGuestCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(CheckOutGuestCommand request, CancellationToken cancellationToken)
        {
            var stay = await _context.Stays.FindAsync([request.StayId], cancellationToken);
            if (stay is null)
                return Result.Failure(Error.NotFound("Stay.NotFound", "Stay not found."));

            var room = await _context.Rooms.FindAsync([stay.RoomId], cancellationToken);
            if (room is null)
                return Result.Failure(Error.NotFound("Room.NotFound", "Room not found."));

            var stayResult = stay.CheckOut(DateTimeOffset.UtcNow);
            if (stayResult.IsError)
                return Result.Failure(stayResult.Errors);

            var roomResult = room.CheckOut();
            if (roomResult.IsError)
                return Result.Failure(roomResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}