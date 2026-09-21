using HotelManagementSystem.Domain.Common;

namespace HotelManagementSystem.Domain.Entities.Stays
{
    public sealed class StayCheckedOutEvent(Guid stayId, Guid guestId, Guid roomId, DateTimeOffset checkOutDate) : DomainEvent
    {
        public Guid StayId { get; } = stayId;
        public Guid GuestId { get; } = guestId;
        public Guid RoomId { get; } = roomId;
        public DateTimeOffset CheckOutDate { get; } = checkOutDate;
    }
}