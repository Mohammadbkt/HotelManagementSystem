using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<Result<IssuedRefreshToken>> CreateAsync(Guid userId, string? deviceName, string? ipAddress, CancellationToken ct);

    Task<Result> RevokeAsync(string rawToken, CancellationToken ct);

    Task<Result> RevokeAllForUserAsync(Guid userId, CancellationToken ct);
}