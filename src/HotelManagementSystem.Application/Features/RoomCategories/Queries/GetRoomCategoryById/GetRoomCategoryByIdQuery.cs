using HotelManagementSystem.Application.Features.RoomCategories.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoryById
{
    public sealed record GetRoomCategoryByIdQuery(
        Guid RoomCategoryId
    ) : IRequest<Result<RoomCategoryDto>>;
    
}