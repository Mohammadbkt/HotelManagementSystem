using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.ReturnRoomToService
{
    public sealed class ReturnRoomToServiceCommandHandler(IAppDbContext context) : IRequestHandler<ReturnRoomToServiceCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(ReturnRoomToServiceCommand request, CancellationToken cancellationToken)
        {
             var room = await _context.Rooms.FindAsync([request.RoomId], cancellationToken);
            if (room is null)
                return Result.Failure(ApplicationErrors.Room.NotFound);

            var roomResult = room.ReturnToService();
            if (roomResult.IsError)
                return Result.Failure(roomResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}