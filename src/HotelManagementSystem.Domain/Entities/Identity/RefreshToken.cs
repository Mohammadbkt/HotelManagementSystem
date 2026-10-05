using System.Net;
using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Identity;

public sealed class RefreshToken : AuditableEntity
{
    public string Token { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public DateTimeOffset ExpiresOnUtc { get; private set; }
    public string? DeviceName { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresOnUtc;

    public bool IsActive =>
        RevokedAt is null
        && ReplacedByToken is null
        && !IsExpired;


    private RefreshToken()
    {

    }

    private RefreshToken(Guid id, string token, Guid userId, DateTimeOffset expiresOnUtc, string? deviceName, string? ipAddress)
        : base(id)
    {
        Token = token;
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
        DeviceName = deviceName;
        IpAddress = ipAddress;
    }

    public static Result<RefreshToken> Create(string token, Guid userId, DateTimeOffset expiresOnUtc, string? deviceName, string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Result<RefreshToken>.Failure(RefreshTokenErrors.TokenRequired);

        if (userId == Guid.Empty)
            return Result<RefreshToken>.Failure(RefreshTokenErrors.UserIdRequired);

        if (expiresOnUtc <= DateTimeOffset.UtcNow)
            return Result<RefreshToken>.Failure(RefreshTokenErrors.InvalidExpirationDate);


        return Result<RefreshToken>.Success(new RefreshToken(Guid.NewGuid(), token, userId, expiresOnUtc, deviceName, ipAddress));

    }

    public Result Revoke(DateTimeOffset revokedAt, string? replacedByToken = null)
    {
        if (RevokedAt is not null)
            return Result.Failure(RefreshTokenErrors.AlreadyRevoked);

        if (IsExpired)
            return Result.Failure(RefreshTokenErrors.AlreadyExpired);

        if (revokedAt > DateTimeOffset.UtcNow)
            return Result.Failure(RefreshTokenErrors.InvalidRevocationDate);

        if (replacedByToken is not null && string.IsNullOrWhiteSpace(replacedByToken))
            return Result.Failure(RefreshTokenErrors.InvalidReplacedByToken);

        RevokedAt = revokedAt;
        ReplacedByToken = replacedByToken;
        return Result.Success();
    }


}
