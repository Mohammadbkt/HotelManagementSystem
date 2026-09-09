using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;
using MediatR;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.UpdateBasePriceForRoomCategory
{
    public sealed class UpdateRoomCategoryBasePriceCommandHandler(IAppDbContext context) : IRequestHandler<UpdateRoomCategoryBasePriceCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result> Handle(UpdateRoomCategoryBasePriceCommand request, CancellationToken cancellationToken)
        {
            var roomCategory = await _context.RoomCategories.FindAsync(request.RoomCategoryId, cancellationToken);
            if (roomCategory is null)
                return Result.Failure(Error.NotFound("RoomCategory.NotFound", " Room category is not found"));

            var moneyResult = Money.Create(request.Price, request.Currency);
            if (moneyResult.IsError)
                return Result.Failure(moneyResult.Errors);

            var updateResult = roomCategory.UpdateBasePrice(moneyResult.Value);
            if (updateResult.IsError)
                return Result.Failure(updateResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}