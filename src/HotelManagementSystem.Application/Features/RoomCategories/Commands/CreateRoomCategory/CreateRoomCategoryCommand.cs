using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.CreateRoomCategory
{
    public sealed record CreateRoomCategoryCommand(
        Guid HotelId,
        string Name,
        string? Description,
        int MaxCapacity,
        decimal BasePrice,
        string Currency
    ) : IRequest<Result<Guid>>;
    
}