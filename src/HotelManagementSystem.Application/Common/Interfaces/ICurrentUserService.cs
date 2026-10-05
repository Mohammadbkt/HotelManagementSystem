namespace HotelManagementSystem.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserName { get; }
        Guid? HotelId { get; }
        IReadOnlyCollection<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}