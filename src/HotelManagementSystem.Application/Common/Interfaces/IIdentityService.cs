using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Employee;

namespace HotelManagementSystem.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<AppUserDto>> CreateUserAsync(string email, string username, string password, string? firstName, string? lastName, string? phoneNumber, string? role, CancellationToken cancellationToken);
    Task<bool> IsInRoleAsync(Guid userId, EmployeeRole role);
    // Task<bool> AuthorizeAsync(Guid userId, string? policyName);
    Task<Result<AppUserDto>> AuthenticateAsync(string identifier, string password, CancellationToken cancellationToken);
    Task<Result<AppUserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<string?> GetUserNameAsync(Guid userId);
}
