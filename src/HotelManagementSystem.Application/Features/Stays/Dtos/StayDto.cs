namespace HotelManagementSystem.Application.Features.Stays.Dtos
{
    public sealed record StayDto(
        Guid Id,
        Guid GuestId,
        Guid RoomId,
        Guid? ReservationId,
        DateTimeOffset CheckInDate,
        DateTimeOffset? CheckOutDate
    );
}