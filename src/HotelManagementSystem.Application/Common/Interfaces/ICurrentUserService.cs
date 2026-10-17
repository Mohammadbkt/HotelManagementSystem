using HotelManagementSystem.Domain.ValueObjects.Email;

namespace HotelManagementSystem.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserName { get; }
        Guid? HotelId { get; }
        EmailAddress Email { get; }
        IReadOnlyCollection<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}