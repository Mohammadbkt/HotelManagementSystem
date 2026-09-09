using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.RoomCategories.Commands.UpdateBasePriceForRoomCategory
{
    public sealed record UpdateRoomCategoryBasePriceCommand(
        Guid RoomCategoryId,
        decimal Price,
        string Currency
    ) : IRequest<Result>;
    
}