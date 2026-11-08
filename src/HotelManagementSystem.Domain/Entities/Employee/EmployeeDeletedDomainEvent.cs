using HotelManagementSystem.Domain.Common;

namespace HotelManagementSystem.Domain.Entities.Employee
{
    public sealed class EmployeeDeletedDomainEvent(Guid employeeId, Guid userId, Guid deletedBy) : DomainEvent
    {
        public Guid EmployeeId { get; } = employeeId;
        public Guid UserId { get; } = userId;
        public Guid DeletedBy { get; } = deletedBy;
    }
}