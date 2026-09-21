using HotelManagementSystem.Application.Common.Errors;
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
                return Result<Guid>.Failure(ApplicationErrors.Hotel.NotFound);

            var roomNumberExists = await _context.Rooms
            .AnyAsync(r => r.RoomNumber == request.RoomNumber &&
                           r.HotelId == request.HotelId
                           , cancellationToken);
            
            if (roomNumberExists)
                return Result<Guid>.Failure(ApplicationErrors.Room.AlreadyExists);

            var roomCategory = await _context.RoomCategories
                .FindAsync([request.RoomCategoryId], cancellationToken);
           
            if (roomCategory is null)
                return Result<Guid>.Failure(ApplicationErrors.RoomCategory.NotFound);

            if (roomCategory.HotelId != request.HotelId)
                return Result<Guid>.Failure(ApplicationErrors.RoomCategory.NotFound);

            var roomResult = Room.Create(request.HotelId, request.RoomNumber, roomCategory.Id);
            
            if (roomResult.IsError)
                return Result<Guid>.Failure(roomResult.Errors);


            _context.Rooms.Add(roomResult.Value);

            await _context.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(roomResult.Value.Id);
        }
    }
}