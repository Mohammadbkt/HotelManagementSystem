using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Application.Common.Interfaces;

public interface IUserService
{
    Task<Result<AppUserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<Result<AppUserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<AppUserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Result> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken);
    Task<Result> AssignRoleAsync(Guid userId, string role, CancellationToken cancellationToken);
    Task<Result> RemoveFromRoleAsync(Guid userId, string role, CancellationToken cancellationToken);
    Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken);
    Task<bool> RoleExistsAsync(string role, CancellationToken cancellationToken);
    Task<Result> RequestPasswordResetAsync(string email, CancellationToken ct);
    Task<Result> ResetPasswordByTokenAsync(string email, string token, string newPassword, CancellationToken ct);
    Task<Result> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken);

    public Task<Result> SendInviteAsync(string email, CancellationToken cancellationToken);
    Task<Result> SendEmailConfirmationAsync(string email, CancellationToken cancellationToken);

    Task<Result> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken);
}
