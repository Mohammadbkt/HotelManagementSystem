using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Identity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Identity.Commands.Login
{
    public sealed class LoginCommandHandler(
            IIdentityService identity,
            IRefreshTokenService refreshTokenService,
            IAccessTokenGenerator jwtService,
            ILogger<LoginCommandHandler> logger
            ) : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IIdentityService _identity = identity;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
        private readonly IAccessTokenGenerator _jwtService = jwtService;
        private readonly ILogger<LoginCommandHandler> _logger = logger;

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var authResult = await _identity.AuthenticateAsync(request.Identifier, request.Password, cancellationToken);
            if (authResult.IsError)
            {
                _logger.LogWarning("Failed login for identifier {Identifier}. Errors: {Errors}",
                        request.Identifier, authResult.Errors);
                return Result<AuthResponseDto>.Failure(authResult.Errors);
            }

            if (authResult.Value is null)
                return Result<AuthResponseDto>.Failure(Error.Validation());


            var user = authResult.Value;

            var accessTokenResult = _jwtService.Generate(user);
            if (accessTokenResult.IsError)
            {
                _logger.LogError("Access token generation failed: {Error}", accessTokenResult.TopError.Description);
                return Result<AuthResponseDto>.Failure(accessTokenResult.Errors);
            }

            var refreshTokenResult = await _refreshTokenService.CreateAsync(user.Id, request.DeviceName, request.IpAddress, cancellationToken);
            if (refreshTokenResult.IsError)
            {
                _logger.LogError("Refresh token generation failed: {Error}", refreshTokenResult.TopError.Description);
                return Result<AuthResponseDto>.Failure(refreshTokenResult.Errors);
            }

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return Result<AuthResponseDto>.Success(
                new AuthResponseDto(
            accessTokenResult.Value.Value,
            refreshTokenResult.Value.Token,
            accessTokenResult.Value.ExpiresAtUtc,
            refreshTokenResult.Value.ExpiresOnUtc
            ));



        }
    }
}