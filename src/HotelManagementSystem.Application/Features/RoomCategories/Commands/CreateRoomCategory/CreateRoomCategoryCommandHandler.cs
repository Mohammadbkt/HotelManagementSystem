using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using HotelManagementSystem.Domain.ValueObjects.Money;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.CreateRoomCategory
{
    public sealed class CreateRoomCategoryCommandHandler(IAppDbContext context, ILogger<CreateRoomCategoryCommandHandler> logger) : IRequestHandler<CreateRoomCategoryCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<CreateRoomCategoryCommandHandler> _logger = logger;
        public async Task<Result<Guid>> Handle(CreateRoomCategoryCommand request, CancellationToken cancellationToken)
        {
            var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == request.HotelId, cancellationToken);
            if (hotelExists is false)
            {
                _logger.LogWarning("Hotel {HotelId} not found while creating room category", request.HotelId);
                return Result<Guid>.Failure(ApplicationErrors.Hotel.NotFound);
            }

            var categoryExists = await _context.RoomCategories.AnyAsync(
                rc => rc.HotelId == request.HotelId && rc.Name == request.Name,
                cancellationToken);
            if (categoryExists)
            {
                _logger.LogWarning("Room category already exists for hotel {HotelId}: {Name}", request.HotelId, request.Name);
                return Result<Guid>.Failure(ApplicationErrors.RoomCategory.AlreadyExists);
            }


            var moneyResult = Money.Create(request.BasePrice, request.Currency);
            if (moneyResult.IsError)
            {
                _logger.LogWarning("Invalid base price for room category in hotel {HotelId}: {Errors}", request.HotelId, moneyResult.Errors);
                return Result<Guid>.Failure(moneyResult.Errors);
            }

            var roomCategoryResult = RoomCategory.Create(request.HotelId, request.Name, request.Description, request.MaxCapacity, moneyResult.Value);

            if (roomCategoryResult.IsError)
            {
                _logger.LogWarning("Failed to create room category for hotel {HotelId}: {Errors}", request.HotelId, roomCategoryResult.Errors);
                return Result<Guid>.Failure(roomCategoryResult.Errors);
            }

            await _context.RoomCategories.AddAsync(roomCategoryResult.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Room category {RoomCategoryId} created for hotel {HotelId}", roomCategoryResult.Value.Id, request.HotelId);
            return Result<Guid>.Success(roomCategoryResult.Value.Id);
        }
    }
}