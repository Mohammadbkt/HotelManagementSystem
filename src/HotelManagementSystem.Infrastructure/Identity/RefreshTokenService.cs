using System.Buffers.Text;
using System.Security.Cryptography;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HotelManagementSystem.Infrastructure.Identity
{
    public sealed class RefreshTokenService(
        IAppDbContext context,
        IOptions<JwtOptions> options,
        ILogger<RefreshTokenService> logger) : IRefreshTokenService
    {
        private readonly IAppDbContext _context = context;
        private readonly IOptions<JwtOptions> _options = options;
        private readonly ILogger<RefreshTokenService> _logger = logger;

        public async Task<Result<IssuedRefreshToken>> CreateAsync(Guid userId, string? deviceName, string? ipAddress, CancellationToken cancellationToken)
        {

            var rawToken = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(64));
            var expiry = DateTimeOffset.UtcNow.AddDays(_options.Value.RefreshTokenDays);

            var result = RefreshToken.Create(
                rawToken,
                userId,
                expiry,
                deviceName,
                ipAddress);

            if (result.IsError)
            {
                return Result<IssuedRefreshToken>.Failure(result.Errors);
            }

            _context.RefreshTokens.Add(result.Value);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<IssuedRefreshToken>.Success(new IssuedRefreshToken(rawToken, expiry));
        }

        public async Task<Result> RevokeAllForUserAsync(Guid userId, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;

            var active = await _context.RefreshTokens
                        .Where(t => t.UserId == userId && t.RevokedAt == null && t.ReplacedByTokenHash == null && t.ExpiresOnUtc > now)
                        .ToListAsync();

            if (active.Count == 0)
                return Result.Success();

            foreach (var token in active)
                token.Revoke(now);

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Revoked {Count} refresh token(s) for user {UserId}.",
                active.Count, userId);

            return Result.Success();
        }

        public async Task<Result> RevokeAsync(string rawToken, CancellationToken ct)
        {
            var hash = RefreshToken.HashToken(rawToken);

            var stored = await _context.RefreshTokens
                            .FirstOrDefaultAsync(t => t.TokenHash == hash);

            if (stored is null)
                return Result.Success();

            var revokeResult = stored.Revoke(DateTimeOffset.UtcNow);

            if (!revokeResult.IsSuccess)
                return Result.Success();

            await _context.SaveChangesAsync(ct);
            return Result.Success();

        }
    }
}