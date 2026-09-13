using HotelManagementSystem.Domain.Entities.Rooms;

namespace HotelManagementSystem.Application.Features.Rooms.Dtos
{
    public record RoomDto(
         Guid RoomId,
        string RoomNumber,
        Guid HotelId,
        Guid RoomCategoryId,
        RoomStatus Status,
        string? StatusReason
    );
}