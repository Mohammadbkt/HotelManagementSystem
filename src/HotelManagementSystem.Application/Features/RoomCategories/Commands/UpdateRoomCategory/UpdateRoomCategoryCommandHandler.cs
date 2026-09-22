using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.UpdateRoomCategory
{
    public sealed class UpdateRoomCategoryCommandHandler(IAppDbContext context, ILogger<UpdateRoomCategoryCommandHandler> logger) : IRequestHandler<UpdateRoomCategoryCommand, Result>
    { 
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateRoomCategoryCommandHandler> _logger = logger;
        public async Task<Result> Handle(UpdateRoomCategoryCommand request, CancellationToken cancellationToken)
        {
            var roomCategory = await _context.RoomCategories.FindAsync([request.RoomCategoryId], cancellationToken);
            if(roomCategory is null)
            {
                _logger.LogWarning("Room category {RoomCategoryId} not found", request.RoomCategoryId);
                return Result.Failure(ApplicationErrors.RoomCategory.NotFound);
            }

            var result = roomCategory.UpdateDetails(request.Name, request.Description, request.MaxCapacity);
            if(result.IsError)
            {
                _logger.LogWarning("Failed to update room category {RoomCategoryId}: {Errors}", request.RoomCategoryId, result.Errors);
                return Result.Failure(result.Errors);
            }
            
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Room category {RoomCategoryId} updated", request.RoomCategoryId);
            return Result.Success();
            
        }
    }
}