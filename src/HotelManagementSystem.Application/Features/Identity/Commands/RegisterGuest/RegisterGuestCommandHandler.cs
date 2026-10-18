
using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Identity.Commands.RegisterGuest;

public sealed class RegisterGuestCommandHandler(
    IAppDbContext context,
    IIdentityService identity,
    IRefreshTokenService refreshTokenService,
    IAccessTokenGenerator jwtService,
    ILogger<RegisterGuestCommandHandler> logger)
    : IRequestHandler<RegisterGuestCommand, Result<AuthResponseDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identity = identity;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly IAccessTokenGenerator _jwtService = jwtService;
    private readonly ILogger<RegisterGuestCommandHandler> _logger = logger;

    public async Task<Result<AuthResponseDto>> Handle(
        RegisterGuestCommand request,
        CancellationToken cancellationToken)
    {
        var emailResult = EmailAddress.Create(request.Email);

        if (emailResult.IsError)
            return Result<AuthResponseDto>.Failure(emailResult.Errors);

        PhoneNumber? phone = null;

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneResult = PhoneNumber.Create(request.PhoneNumber);

            if (phoneResult.IsError)
                return Result<AuthResponseDto>.Failure(phoneResult.Errors);

            phone = phoneResult.Value;
        }

        var email = emailResult.Value.Value;
        var phoneValue = phone?.Value;

        var matches = await _context.Guests
            .Where(g =>
                (g.Email != null && g.Email.Value.Value == email) ||
                (phoneValue != null &&
                 g.PhoneNumber != null &&
                 g.PhoneNumber.Value.Value == phoneValue))
            .Take(2)
            .ToListAsync(cancellationToken);

        if (matches.Count > 1)
            return Result<AuthResponseDto>.Failure(GuestErrors.AmbiguousMatch);

        var guest = matches.SingleOrDefault();

        if (guest is not null && guest.UserId is not null)
            return Result<AuthResponseDto>.Failure(ApplicationErrors.GuestErrors.AlreadyExists);

        var isNewGuest = guest is null;

        if (guest is null)
        {
            var guestResult = Guest.Create(
                request.FirstName,
                request.LastName,
                emailResult.Value,
                phone,
                request.DateOfBirth);

            if (guestResult.IsError)
                return Result<AuthResponseDto>.Failure(guestResult.Errors);

            guest = guestResult.Value;
        }

        var userResult = await _identity.CreateUserAsync(
            email,
            request.Username,
            request.Password,
            request.FirstName,
            request.LastName,
            phoneValue,
            Roles.Guest,
            cancellationToken);

        if (userResult.IsError)
        {
            _logger.LogWarning(
                "Guest registration failed for {Email}: {Error}",
                email,
                userResult.TopError.Description);

            return Result<AuthResponseDto>.Failure(userResult.Errors);
        }

        var user = userResult.Value;

        var linkResult = guest.LinkToUserAccount(user.Id);

        if (linkResult.IsError)
        {
            var deleteResult = await _identity.DeleteUserAsync(
                user.Id, CancellationToken.None);

            if (deleteResult.IsError)
            {
                _logger.LogCritical(
                    "Could not delete identity user {UserId} after guest linking failed.",
                    user.Id);
            }

            return Result<AuthResponseDto>.Failure(linkResult.Errors);
        }

        var accessTokenResult = _jwtService.Generate(user);

        if (accessTokenResult.IsError)
        {
            var deleteResult = await _identity.DeleteUserAsync(
                user.Id, CancellationToken.None);

            if (deleteResult.IsError)
            {
                _logger.LogCritical(
                    "Could not delete identity user {UserId} after access-token generation failed.",
                    user.Id);
            }

            return Result<AuthResponseDto>.Failure(
                accessTokenResult.Errors);
        }

        var refreshTokenResult = _refreshTokenService.Create(
            user.Id,
            request.DeviceName,
            request.IpAddress);

        if (refreshTokenResult.IsError)
        {
            var deleteResult = await _identity.DeleteUserAsync(
                user.Id, CancellationToken.None);

            if (deleteResult.IsError)
            {
                _logger.LogCritical(
                    "Could not delete identity user {UserId} after refresh-token creation failed.",
                    user.Id);
            }

            return Result<AuthResponseDto>.Failure(
                refreshTokenResult.Errors);
        }

        if (isNewGuest)
            _context.Guests.Add(guest);

        _context.RefreshTokens.Add(refreshTokenResult.Value);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Application database save failed during registration for user {UserId}.",
                user.Id);

            var deleteResult = await _identity.DeleteUserAsync(
                user.Id, CancellationToken.None);

            if (deleteResult.IsError)
            {
                _logger.LogCritical(
                    "Could not compensate for failed registration of user {UserId}.",
                    user.Id);
            }

            return Result<AuthResponseDto>.Failure(
                Error.Failure(
                    "Register.SaveFailed",
                    "Registration could not be completed. Please try again."));
        }

        _logger.LogInformation(
            "Guest registration completed for user {UserId}, guest {GuestId}.",
            user.Id,
            guest.Id);

        return Result<AuthResponseDto>.Success(
            new AuthResponseDto(
                accessTokenResult.Value.Value,
                refreshTokenResult.Value.Token,
                accessTokenResult.Value.ExpiresAtUtc,
                refreshTokenResult.Value.ExpiresOnUtc));
    }
}
