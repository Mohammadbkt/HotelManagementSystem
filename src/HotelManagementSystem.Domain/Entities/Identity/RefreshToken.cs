using System.Security.Cryptography;
using System.Text;

using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Identity;

public sealed class RefreshToken : AuditableEntity
{
    public string TokenHash { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public DateTimeOffset ExpiresOnUtc { get; private set; }
    public string? DeviceName { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresOnUtc;

    public bool IsActive =>
        RevokedAt is null
        && ReplacedByTokenHash is null
        && !IsExpired;


    private RefreshToken()
    {

    }

    private RefreshToken(Guid id, string tokenHash, Guid userId, DateTimeOffset expiresOnUtc, string? deviceName, string? ipAddress)
        : base(id)
    {
        TokenHash = tokenHash;
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
        DeviceName = deviceName;
        IpAddress = ipAddress;
    }

    public static Result<RefreshToken> Create(string rawToken, Guid userId, DateTimeOffset expiresOnUtc, string? deviceName, string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            return Result<RefreshToken>.Failure(RefreshTokenErrors.TokenRequired);

        if (userId == Guid.Empty)
            return Result<RefreshToken>.Failure(RefreshTokenErrors.UserIdRequired);

        if (expiresOnUtc <= DateTimeOffset.UtcNow)
            return Result<RefreshToken>.Failure(RefreshTokenErrors.InvalidExpirationDate);

        var hash = HashToken(rawToken);

        return Result<RefreshToken>.Success(new RefreshToken(Guid.NewGuid(), hash, userId, expiresOnUtc, deviceName, ipAddress));

    }

    public Result Revoke(DateTimeOffset revokedAt, string? replacedByRawToken = null)
    {
        if (RevokedAt is not null)
            return Result.Failure(RefreshTokenErrors.AlreadyRevoked);

        if (IsExpired)
            return Result.Failure(RefreshTokenErrors.AlreadyExpired);

        if (revokedAt > DateTimeOffset.UtcNow)
            return Result.Failure(RefreshTokenErrors.InvalidRevocationDate);

        if (replacedByRawToken is not null && string.IsNullOrWhiteSpace(replacedByRawToken))
            return Result.Failure(RefreshTokenErrors.InvalidReplacedByToken);

        RevokedAt = revokedAt;

        ReplacedByTokenHash = replacedByRawToken is null
            ? null
            : HashToken(replacedByRawToken);

        return Result.Success();
    }

    public static string HashToken(string rawToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawToken);

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }


}
