namespace HotelManagementSystem.Domain.Common
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; }
        DateTimeOffset? DeletedAt { get; }
        Guid? DeletedBy { get; }

        void Delete(Guid deletedBy);
        void Restore();
    }
}