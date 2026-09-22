using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.UpdateBasePriceForRoomCategory
{
    public sealed class UpdateRoomCategoryBasePriceCommandHandler(IAppDbContext context, ILogger<UpdateRoomCategoryBasePriceCommandHandler> logger) : IRequestHandler<UpdateRoomCategoryBasePriceCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateRoomCategoryBasePriceCommandHandler> _logger = logger;
        public async Task<Result> Handle(UpdateRoomCategoryBasePriceCommand request, CancellationToken cancellationToken)
        {
            var roomCategory = await _context.RoomCategories.FindAsync([request.RoomCategoryId], cancellationToken);
            if (roomCategory is null)
            {
                _logger.LogWarning("Room category {RoomCategoryId} not found", request.RoomCategoryId);
                return Result.Failure(ApplicationErrors.RoomCategory.NotFound);
            }

            var moneyResult = Money.Create(request.Price, request.Currency);
            if (moneyResult.IsError)
            {
                _logger.LogWarning("Invalid base price for room category {RoomCategoryId}: {Errors}", request.RoomCategoryId, moneyResult.Errors);
                return Result.Failure(moneyResult.Errors);
            }

            var updateResult = roomCategory.UpdateBasePrice(moneyResult.Value);
            if (updateResult.IsError)
            {
                _logger.LogWarning("Failed to update price for room category {RoomCategoryId}: {Errors}", request.RoomCategoryId, updateResult.Errors);
                return Result.Failure(updateResult.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Base price updated for room category {RoomCategoryId}", request.RoomCategoryId);
            return Result.Success();
        }
    }
}