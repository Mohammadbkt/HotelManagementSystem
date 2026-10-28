using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using HotelManagementSystem.Infrastructure.Identity;
using HotelManagementSystem.Infrastructure.Identity.Extensions;

namespace HotelManagementSystem.Infrastructure.Identity
{
    public sealed class AuthService(
        UserManager<AppUser> userManager,
        IUserService users,
        IAccessTokenGenerator accessTokens,
        IRefreshTokenService refreshTokens,
        IAppDbContext context,
        ILogger<AuthService> logger) : IAuthService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IUserService _users = users;
        private readonly IAccessTokenGenerator _accessTokens = accessTokens;
        private readonly IRefreshTokenService _refreshTokens = refreshTokens;
        private readonly IAppDbContext _context = context;
        private readonly ILogger<AuthService> _logger = logger;


        public async Task<Result<AuthResponseDto>> LoginAsync(
    string email, string password, string? deviceName, string? ipAddress, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null || user.IsDeleted)
                return Result<AuthResponseDto>.Failure(ApplicationErrors.Auth.InvalidCredentials);

            if (await _userManager.IsLockedOutAsync(user))
                return Result<AuthResponseDto>.Failure(ApplicationErrors.Auth.LockedOut);

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                await _userManager.AccessFailedAsync(user);
                return Result<AuthResponseDto>.Failure(ApplicationErrors.Auth.InvalidCredentials);
            }

            if (!user.EmailConfirmed)
                return Result<AuthResponseDto>.Failure(ApplicationErrors.Auth.EmailNotConfirmed);

            await _userManager.ResetAccessFailedCountAsync(user);

            return await IssueTokensAsync(user, deviceName, ipAddress, ct);
        }


        public async Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken)
        {
            return await _refreshTokens.RevokeAsync(refreshToken, cancellationToken);
        }

        public Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, string? deviceName, string? ipAddress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<Result<AuthResponseDto>> IssueTokensAsync(
            AppUser user, string? deviceName, string? ipAddress, CancellationToken ct)
        {
            var userDto = await user.ToDtoAsync(_userManager, _context, ct);

            var access = _accessTokens.Generate(userDto);
            if (access.IsError)
            {
                _logger.LogError("Access token generation failed for user {UserId}: {Error}", user.Id, access.TopError.Code);
                return Result<AuthResponseDto>.Failure(access.Errors);
            }

            var refresh = await _refreshTokens.CreateAsync(user.Id, deviceName, ipAddress, ct);
            if (refresh.IsError)
                return Result<AuthResponseDto>.Failure(refresh.Errors);

            return Result<AuthResponseDto>.Success(new AuthResponseDto(
                access.Value.Value, refresh.Value.RawToken,
                access.Value.ExpiresAtUtc, refresh.Value.ExpiresOnUtc, userDto));
        }

    }
}