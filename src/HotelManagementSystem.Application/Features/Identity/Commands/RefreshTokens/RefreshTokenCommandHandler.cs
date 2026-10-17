using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Identity.Commands.RefreshTokens;

public sealed class RefreshTokenCommandHandler(
    IAppDbContext context,
    IIdentityService identityService,
    IAccessTokenGenerator jwtService,
    IRefreshTokenService refreshTokenService,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;
    private readonly IAccessTokenGenerator _jwtService = jwtService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;

    public async Task<Result<AuthResponseDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var oldToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.Token == request.RefreshToken,
                cancellationToken);

        if (oldToken is null || oldToken.ExpiresOnUtc <= now)
        {
            return Result<AuthResponseDto>.Failure(
                Error.Validation(
                    "Auth.InvalidRefreshToken",
                    "The refresh token is invalid or expired."));
        }

        if (oldToken.RevokedAt is not null)
        {
            await _context.RefreshTokens
                .Where(x =>
                    x.UserId == oldToken.UserId &&
                    x.RevokedAt == null &&
                    x.ExpiresOnUtc > now)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(x => x.RevokedAt, now),
                    cancellationToken);


            _logger.LogWarning(
                "Refresh token reuse detected for user {UserId}.",
                oldToken.UserId);

            return Result<AuthResponseDto>.Failure(
                Error.Validation(
                    "Auth.InvalidRefreshToken",
                    "The refresh token is invalid."));
        }

        var userResult = await _identityService.GetUserByIdAsync(
            oldToken.UserId,
            cancellationToken);

        if (userResult.IsError)
        {
            return Result<AuthResponseDto>.Failure(
                Error.Validation(
                    "Auth.InvalidRefreshToken",
                    "The refresh token is invalid."));
        }

        var user = userResult.Value;

        var accessTokenResult = _jwtService.Generate(user);

        if (accessTokenResult.IsError)
        {
            return Result<AuthResponseDto>.Failure(
                accessTokenResult.Errors);
        }

        var newRefreshTokenResult = _refreshTokenService.Create(user.Id, request.DeviceName, request.IpAddress);

        if (newRefreshTokenResult.IsError)
        {
            return Result<AuthResponseDto>.Failure(
                newRefreshTokenResult.Errors);
        }

        var accessToken = accessTokenResult.Value;
        var newRefreshToken = newRefreshTokenResult.Value;

        oldToken.Revoke(now, newRefreshToken.Token);

        await _context.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AuthResponseDto>.Success(
            new AuthResponseDto(
                accessToken.Value,
                newRefreshToken.Token,
                accessToken.ExpiresAtUtc,
                newRefreshToken.ExpiresOnUtc));
    }
}
