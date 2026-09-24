using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Identity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Identity.Commands.Login
{
    public sealed class LoginCommandHandler(
            IAppDbContext context,
            IIdentityService identity,
            IRefreshTokenService refreshTokenService,
            IJwtService jwtService,
            ILogger<LoginCommandHandler> logger
            ) : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IIdentityService _identity = identity;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
        private readonly IJwtService _jwtService = jwtService;
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

            var accessTokenResult = await _jwtService.GenerateAccessToken(user, cancellationToken);
            if (accessTokenResult.IsError)
            {
                _logger.LogError("Access token generation failed: {Error}", accessTokenResult.TopError.Description);
                return Result<AuthResponseDto>.Failure(accessTokenResult.Errors);
            }

            var refreshTokenValueResult = await _refreshTokenService.CreateAsync(user.Id, cancellationToken);
            if (refreshTokenValueResult.IsError)
            {
                _logger.LogError("Refresh token generation failed: {Error}", refreshTokenValueResult.TopError.Description);
                return Result<AuthResponseDto>.Failure(refreshTokenValueResult.Errors);
            }

            var refreshTokenResult = RefreshToken
            .Create(
                refreshTokenValueResult.Value.Value,
                user.Id,
                refreshTokenValueResult.Value.ExpiresAt,
                request.DeviceName ,
                request.IpAddress);

            if (refreshTokenResult.IsError)
                return Result<AuthResponseDto>.Failure(refreshTokenResult.Errors);

            await _context.RefreshTokens.AddAsync(refreshTokenResult.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);


            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return Result<AuthResponseDto>.Success(
                new AuthResponseDto(
            accessTokenResult.Value.Value,
            refreshTokenValueResult.Value.Value,
            accessTokenResult.Value.ExpiresAt
            ));



        }
    }
}