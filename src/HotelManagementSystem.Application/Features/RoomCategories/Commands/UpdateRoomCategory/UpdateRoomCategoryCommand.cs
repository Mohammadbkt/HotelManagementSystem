using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.UpdateRoomCategory
{
    public sealed record UpdateRoomCategoryCommand(
        Guid RoomCategoryId,
        string Name, 
        string? Description, 
        int MaxCapacity
    ) : IRequest<Result>;
}