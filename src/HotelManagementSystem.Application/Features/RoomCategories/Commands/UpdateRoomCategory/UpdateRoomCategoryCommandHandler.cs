using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.UpdateRoomCategory
{
    public sealed class UpdateRoomCategoryCommandHandler(IAppDbContext context) : IRequestHandler<UpdateRoomCategoryCommand, Result>
    { 
        private readonly IAppDbContext _context = context;
        public async Task<Result> Handle(UpdateRoomCategoryCommand request, CancellationToken cancellationToken)
        {
            var roomCategory = await _context.RoomCategories.FindAsync(request.RoomCategoryId, cancellationToken);
            if(roomCategory is null)
                return Result.Failure(Error.NotFound());

            var result = roomCategory.UpdateDetails(request.Name, request.Description, request.MaxCapacity);
            if(result.IsError)
                return Result.Failure(result.Errors);
            
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
            
        }
    }
}