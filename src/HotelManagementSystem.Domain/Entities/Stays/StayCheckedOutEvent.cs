using HotelManagementSystem.Domain.Common;

namespace HotelManagementSystem.Domain.Entities.Stays
{
    public sealed class StayCheckedOutEvent: DomainEvent
    {
        public Guid StayId { get; }
        public Guid GuestId { get; }
        public Guid RoomId { get; }
        public DateTimeOffset CheckOutDate { get; }

        public StayCheckedOutEvent(Guid stayId, Guid guestId, Guid roomId, DateTimeOffset checkOutDate)
        {
            StayId = stayId;
            GuestId = guestId;
            RoomId = roomId;
            CheckOutDate = checkOutDate;
        }
    }
}