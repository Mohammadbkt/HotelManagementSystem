namespace HotelManagementSystem.Application.Features.RoomCategories.Dtos
{
    public sealed record RoomCategoryDto(
        Guid Id,
        Guid HotelId,
        string Name,
        string? Description,
        int MaxCapacity,
        decimal BasePricePerNight,
        string Currency
    );

}