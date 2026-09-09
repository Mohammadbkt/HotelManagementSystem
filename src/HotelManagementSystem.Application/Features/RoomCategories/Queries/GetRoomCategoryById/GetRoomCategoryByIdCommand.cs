using HotelManagementSystem.Application.Features.RoomCategories.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using MediatR;

namespace HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoryById
{
    public sealed record GetRoomCategoryByIdCommand(
        Guid RoomCategoryId
    ) : IRequest<Result<RoomCategoryDto>>;
    
}