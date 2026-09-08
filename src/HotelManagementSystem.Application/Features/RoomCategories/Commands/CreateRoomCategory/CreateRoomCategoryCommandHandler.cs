using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using HotelManagementSystem.Domain.ValueObjects.Money;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.CreateRoomCategory
{
    public sealed class CreateRoomCategoryCommandHandler(IAppDbContext context) : IRequestHandler<CreateRoomCategoryCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<Guid>> Handle(CreateRoomCategoryCommand request, CancellationToken cancellationToken)
        {
            var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == request.HotelId, cancellationToken);
            if (hotelExists is false)
                return Result<Guid>.Failure(Error.NotFound("Hotel.NotFound", "Hotel does not exist."));

            var categoryExists = await _context.RoomCategories.AnyAsync(
                rc => rc.HotelId == request.HotelId && rc.Name == request.Name,
                cancellationToken);
            if (categoryExists)
                return Result<Guid>.Failure(Error.Conflict("RoomCategory.DuplicateName", "A room category with this name already exists for this hotel."));


            var MoneyResult = Money.Create(request.BasePrice, request.Currency);
            if (MoneyResult.IsError)
                return Result<Guid>.Failure(MoneyResult.Errors);

            var roomCategoryResult = RoomCategory.Create(request.HotelId, request.Name, request.Description, request.MaxCapacity, MoneyResult.Value);

            if (roomCategoryResult.IsError)
                return Result<Guid>.Failure(Error.Unexpected());

            await _context.RoomCategories.AddAsync(roomCategoryResult.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(roomCategoryResult.Value.Id);
        }
    }
}