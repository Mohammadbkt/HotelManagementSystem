using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Identity;
using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Identity.Commands.Register
{
    public sealed class RegisterCommandHandler(
            IAppDbContext context,
            IIdentityService identity,
            IRefreshTokenService refreshTokenService,
            IAccessTokenGenerator jwtService,
            ILogger<RegisterCommandHandler> logger
            ) : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IIdentityService _identity = identity;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
        private readonly IAccessTokenGenerator _jwtService = jwtService;
        private readonly ILogger<RegisterCommandHandler> _logger = logger;


        public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var emailResult = EmailAddress.Create(request.Email);
            if (emailResult.IsError)
                return Result<AuthResponseDto>.Failure(emailResult.Errors);

            var createUserResult = await _identity.CreateUserAsync(request.Email, request.Username, request.Password,request.FirstName,request.LastName,request.PhoneNumber,null,  cancellationToken);
            if (createUserResult.IsError)
            {
                _logger.LogWarning("Registration failed for {Email}: {Error}", request.Email, createUserResult.TopError.Description);
                return Result<AuthResponseDto>.Failure(createUserResult.Errors);
            }

            var user = createUserResult.Value;

            var existingGuest = await _context.Guests
                            .FirstOrDefaultAsync(g => g.Email!.Value.Value == request.Email, cancellationToken);
            if (existingGuest is not null)
            {
                var linkResult = existingGuest.LinkToUserAccount(user.Id);
                if (linkResult.IsError)
                    return Result<AuthResponseDto>.Failure(linkResult.Errors);
            }
            else if (request.PhoneNumber is not null)
            {
                var phoneNumberResult = PhoneNumber.Create(request.PhoneNumber);

                var guestResult = Guest.Create(
                    request.FirstName, request.LastName, emailResult.Value, phoneNumberResult.Value, request.DateOfBirth);
                if (guestResult.IsError)
                    return Result<AuthResponseDto>.Failure(guestResult.Errors);

                guestResult.Value.LinkToUserAccount(user.Id);
                _context.Guests.Add(guestResult.Value);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var accessTokenResult = _jwtService.Generate(user);
            if (accessTokenResult.IsError)
                return Result<AuthResponseDto>.Failure(accessTokenResult.Errors);

            var refreshTokenValueResult = await _refreshTokenService.CreateAsync(user.Id,request.DeviceName, request.IpAddress, cancellationToken);
            if (refreshTokenValueResult.IsError)
                return Result<AuthResponseDto>.Failure(refreshTokenValueResult.Errors);

            var refreshTokenResult = RefreshToken.Create(
                refreshTokenValueResult.Value.Token, user.Id, DateTimeOffset.UtcNow.AddMinutes(15),
                request.DeviceName, request.IpAddress);
            if (refreshTokenResult.IsError)
                return Result<AuthResponseDto>.Failure(refreshTokenResult.Errors);

            _context.RefreshTokens.Add(refreshTokenResult.Value);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("New guest account registered: {Email}", request.Email);

            return Result<AuthResponseDto>.Success(new AuthResponseDto(
                accessTokenResult.Value.Value,
                refreshTokenValueResult.Value.Token,
                accessTokenResult.Value.ExpiresAtUtc,
                refreshTokenValueResult.Value.ExpiresOnUtc));

        }
    }
}