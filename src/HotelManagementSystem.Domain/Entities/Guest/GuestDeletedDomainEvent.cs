using HotelManagementSystem.Domain.Common;

namespace HotelManagementSystem.Domain.Entities.Guest
{
    public sealed class GuestDeletedDomainEvent(Guid guestId, Guid? userId, Guid deletedBy) : DomainEvent
    {
        public Guid GuestId { get; } = guestId;
        public Guid? UserId { get; } = userId;
        public Guid DeletedBy { get; } = deletedBy;
    }
}