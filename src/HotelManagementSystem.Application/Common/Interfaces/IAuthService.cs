using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Application.Common.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> LoginAsync(string email, string password, string? deviceName, string? ipAddress, CancellationToken cancellationToken);

    Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, string? deviceName, string? ipAddress, CancellationToken cancellationToken);

    Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken);
}