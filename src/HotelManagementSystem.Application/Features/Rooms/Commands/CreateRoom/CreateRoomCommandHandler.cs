using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Rooms;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.CreateRoom
{
    public sealed class CreateRoomCommandHandler(IAppDbContext context) : IRequestHandler<CreateRoomCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<Guid>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            var hotelExists = await _context.Hotels
                .AnyAsync(h => h.Id == request.HotelId, cancellationToken);

            if (!hotelExists)
                return Result<Guid>.Failure(Error.NotFound("Hotel.NotFound", "Hotel not found."));

            var roomNumberExists = await _context.Rooms
            .AnyAsync(r => r.RoomNumber == request.RoomNumber &&
                           r.HotelId == request.HotelId
                           , cancellationToken);
            
            if (roomNumberExists)
                return Result<Guid>.Failure(Error.Conflict("Room.RoomNumberAlreadyExists", "A room with this number already exists in this hotel."));

            var roomCategory = await _context.RoomCategories
                .FindAsync([request.RoomCategoryId], cancellationToken);
           
            if (roomCategory is null)
                return Result<Guid>.Failure(Error.NotFound("RoomCategory.NotFound", "Room category not found"));

            if (roomCategory.HotelId != request.HotelId)
                return Result<Guid>.Failure(Error.Validation("Room.CategoryHotelMismatch", "The selected room category does not belong to this hotel."));

            var roomResult = Room.Create(request.HotelId, request.RoomNumber, roomCategory.Id);
            
            if (roomResult.IsError)
                return Result<Guid>.Failure(roomResult.Errors);


            _context.Rooms.Add(roomResult.Value);

            await _context.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(roomResult.Value.Id);
        }
    }
}