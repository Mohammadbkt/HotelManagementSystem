using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Rooms;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Rooms.Commands.CreateRoom
{
    public sealed class CreateRoomCommandHandler(IAppDbContext context, ILogger<CreateRoomCommandHandler> logger) : IRequestHandler<CreateRoomCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<CreateRoomCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        {
            var hotelExists = await _context.Hotels
                .AnyAsync(h => h.Id == request.HotelId, cancellationToken);

            if (!hotelExists)
            {
                _logger.LogWarning("Hotel {HotelId} not found while creating room {RoomNumber}", request.HotelId, request.RoomNumber);
                return Result<Guid>.Failure(ApplicationErrors.Hotel.NotFound);
            }

            var roomNumberExists = await _context.Rooms
            .AnyAsync(r => r.RoomNumber == request.RoomNumber &&
                           r.HotelId == request.HotelId
                           , cancellationToken);
            
            if (roomNumberExists)
            {
                _logger.LogWarning("Room number {RoomNumber} already exists for hotel {HotelId}", request.RoomNumber, request.HotelId);
                return Result<Guid>.Failure(ApplicationErrors.Room.AlreadyExists);
            }

            var roomCategory = await _context.RoomCategories
                .FindAsync([request.RoomCategoryId], cancellationToken);
           
            if (roomCategory is null)
            {
                _logger.LogWarning("Room category {RoomCategoryId} not found while creating room {RoomNumber}", request.RoomCategoryId, request.RoomNumber);
                return Result<Guid>.Failure(ApplicationErrors.RoomCategory.NotFound);
            }

            if (roomCategory.HotelId != request.HotelId)
            {
                _logger.LogWarning("Room category {RoomCategoryId} does not belong to hotel {HotelId}", request.RoomCategoryId, request.HotelId);
                return Result<Guid>.Failure(ApplicationErrors.RoomCategory.NotFound);
            }

            var roomResult = Room.Create(request.HotelId, request.RoomNumber, roomCategory.Id);
            
            if (roomResult.IsError)
            {
                _logger.LogWarning("Failed to create room {RoomNumber} in hotel {HotelId}: {Errors}", request.RoomNumber, request.HotelId, roomResult.Errors);
                return Result<Guid>.Failure(roomResult.Errors);
            }


            _context.Rooms.Add(roomResult.Value);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Room {RoomId} created for hotel {HotelId}", roomResult.Value.Id, request.HotelId);
            return Result<Guid>.Success(roomResult.Value.Id);
        }
    }
}