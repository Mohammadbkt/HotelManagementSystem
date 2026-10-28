using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Infrastructure.Email;
using HotelManagementSystem.Infrastructure.Identity.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HotelManagementSystem.Infrastructure.Identity;

public sealed class UserService(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IAppDbContext context,
    IRefreshTokenService refreshToken,
    ICurrentUserService currentUser,
    IEmailSender emailSender,
    IOptions<ResendOptions> resendOptions,
    ILogger<UserService> logger) : IUserService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
    private readonly IAppDbContext _context = context;
    private readonly IRefreshTokenService _refreshToken = refreshToken;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IOptions<ResendOptions> _resendOptions = resendOptions;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<Result> AssignRoleAsync(Guid userId, string role, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogWarning("Cannot assign role '{Role}': user {UserId} not found.", role, userId);
            return Result.Failure(ApplicationErrors.Auth.UserNotFound);
        }

        var roleExists = await _roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            _logger.LogWarning(
                "Failed to add role '{Role}' to user {UserId}: role does not exist.",
                role, userId);

            return Result.Failure(ApplicationErrors.Auth.RoleNotExists);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            _logger.LogWarning(
                "Failed to add role '{Role}' to user {UserId}. Errors: {Errors}.",
                role, userId,
                string.Join("; ", roleResult.Errors.Select(e => $"{e.Code}: {e.Description}")));

            return Result.Failure(roleResult.Errors
                    .Select(e => Error.Validation($"Identity.{e.Code}", e.Description))
                    .ToList());
        }

        var stampResult = await _userManager.UpdateSecurityStampAsync(user);
        if (!stampResult.Succeeded)
        {
            _logger.LogError(
                "Failed to update security stamp after assigning role '{Role}' to user {UserId}: {Errors}",
                role, userId,
                string.Join("; ", stampResult.Errors.Select(e => $"{e.Code}: {e.Description}")));
        }

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
        {
            _logger.LogWarning("Change password failed: user {UserId} not found or inactive.", userId);
            return Result.Failure(ApplicationErrors.Auth.UserNotFound);
        }

        var changeResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!changeResult.Succeeded)
        {
            _logger.LogWarning(
        "Change password failed for user {UserId}. Errors: {Errors}",
        userId,
        string.Join("; ", changeResult.Errors.Select(e => $"{e.Code}: {e.Description}")));
            return Result.Failure(changeResult.Errors.Select(e => Error.Failure(e.Code, e.Description)).ToList());
        }

        var revokeResult = await _refreshToken.RevokeAllForUserAsync(userId, cancellationToken);
        if (!revokeResult.IsSuccess)
        {
            _logger.LogError("Password changed for user {UserId}, but refresh token revocation failed. Errors: {Errors}",
             userId, string.Join("; ", revokeResult.Errors.Select(e => e.Code)));
        }

        return Result.Success();
    }

    public async Task<Result> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Email confirmation failed: missing email or token.");
            return Result.Failure(ApplicationErrors.Auth.InvalidToken);
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.IsDeleted)
        {
            _logger.LogWarning("Email confirmation failed: user {Email} not found or inactive.", email);
            return Result.Failure(ApplicationErrors.Auth.InvalidToken);
        }

        var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
        if (!confirmResult.Succeeded)
        {
            _logger.LogWarning(
                "Email confirmation failed for {Email}. Errors: {Errors}",
                email,
                string.Join("; ", confirmResult.Errors.Select(e => $"{e.Code}: {e.Description}")));
            return Result.Failure(confirmResult.Errors
                        .Select(e => Error.Validation(e.Code, e.Description))
                        .ToList());
        }

        return Result.Success();
    }

    public async Task<Result<AppUserDto>> CreateUserAsync(CreateUserRequest userRequest, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            UserName = userRequest.UserName,
            Email = userRequest.Email,
            PhoneNumber = userRequest.PhoneNumber,
            FirstName = userRequest.FirstName,
            LastName = userRequest.LastName,
            EmailConfirmed = false
        };


        var createResult = string.IsNullOrEmpty(userRequest.Password)
            ? await _userManager.CreateAsync(user)
            : await _userManager.CreateAsync(user, userRequest.Password);

        if (!createResult.Succeeded)
        {
            _logger.LogWarning(
                "User creation failed for email '{Email}'. Errors: {Errors}.",
                userRequest.Email,
                string.Join("; ", createResult.Errors.Select(e => $"{e.Code}: {e.Description}")));

            var errors = createResult.Errors
                .Select(e => Error.Validation(e.Code, e.Description))
                .ToList();

            return Result<AppUserDto>.Failure(errors);
        }


        _logger.LogInformation("User '{UserId}' created successfully.", user.Id);

        var userDto = new AppUserDto(
            Id: user.Id,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email ?? string.Empty,
            UserName: user.UserName ?? string.Empty,
            HotelId: null,
            PhoneNumber: user.PhoneNumber,
            IsActive: true,
            Roles: Array.Empty<string>());

        return Result<AppUserDto>.Success(userDto);
    }



    public async Task<Result> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            _logger.LogWarning("Attempted to deactivate non-existent user {UserId}", userId);
            return Result.Success();
        }

        if (user.IsDeleted)
        {
            return Result.Success();
        }

        user.IsDeleted = true;

        var stampResult = await _userManager.UpdateSecurityStampAsync(user);
        if (!stampResult.Succeeded)
        {
            _logger.LogError(
                "Failed to deactivate user {UserId}: {Errors}",
                userId,
                string.Join("; ", stampResult.Errors.Select(e => $"{e.Code}: {e.Description}")));
            return Result.Failure(Error.Failure());
        }

        var revokeResult = await _refreshToken.RevokeAllForUserAsync(userId, cancellationToken);

        if (!revokeResult.IsSuccess)
        {
            _logger.LogError(
                "User {UserId} deactivated, but refresh token revocation FAILED. " +
                "Tokens remain valid until expiry. Errors: {Errors}",
                userId,
                string.Join("; ", revokeResult.Errors.Select(e => e.Code)));
        }
        else
        {
            _logger.LogInformation(
                "User {UserId} deactivated and refresh tokens revoked.", userId);
        }

        _logger.LogInformation("User {UserId} deactivated and refresh tokens revoked.", userId);

        return Result.Success();
    }

    public async Task<Result<AppUserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result<AppUserDto>.Failure(ApplicationErrors.Auth.UserNotFound);

        var userDto = await user.ToDtoAsync(_userManager, _context, cancellationToken);


        return Result<AppUserDto>.Success(userDto);
    }

    public async Task<Result<AppUserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result<AppUserDto>.Failure(ApplicationErrors.Auth.UserNotFound);
        }

        var userDto = await user.ToDtoAsync(_userManager, _context, cancellationToken);

        return Result<AppUserDto>.Success(userDto);
    }

    public async Task<Result> RemoveFromRoleAsync(Guid userId, string role, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
        {
            _logger.LogWarning(
                "Cannot remove role '{Role}': user {UserId} not found or inactive.",
                role, userId);
            return Result.Failure(ApplicationErrors.Auth.UserNotFound);
        }

        var removeResult = await _userManager.RemoveFromRoleAsync(user, role);
        if (!removeResult.Succeeded)
        {
            _logger.LogWarning(
                "Failed to remove role '{Role}' from user {UserId}. Errors: {Errors}",
                role, userId,
                string.Join("; ", removeResult.Errors.Select(e => $"{e.Code}: {e.Description}")));

            return Result.Failure(removeResult.Errors
                .Select(e => Error.Validation($"Identity.{e.Code}", e.Description))
                .ToList());
        }

        var stampResult = await _userManager.UpdateSecurityStampAsync(user);
        if (!stampResult.Succeeded)
        {
            _logger.LogError(
                "Failed to update security stamp after removing role '{Role}' from user {UserId}: {Errors}",
                role, userId,
                string.Join("; ", stampResult.Errors.Select(e => $"{e.Code}: {e.Description}")));
        }

        _logger.LogInformation("Role '{Role}' removed from user {UserId}.", role, userId);
        return Result.Success();
    }

    public async Task<Result> RequestPasswordResetAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || user.IsDeleted)
        {
            _logger.LogInformation(
                "Password reset skipped for {Email}: no active user.", email);
            return Result.Success();
        }

        var result = await SendTokenEmailAsync(user,
        _userManager.GeneratePasswordResetTokenAsync,
        _resendOptions.Value.PasswordResetUrlFormat,
        "Reset your password",
        "<p>Reset your password by clicking the link below:</p>",
        "Reset password",
        "<p>If you didn't request this, you can ignore this email. Your password will not change unless you click the link above.</p>",
        cancellationToken);

        if (result.IsError)
            _logger.LogWarning("Password reset email failed for user {UserId}.", user.Id);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordByTokenAsync(string email, string token, string newPassword, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(token) ||
        string.IsNullOrWhiteSpace(newPassword))
        {
            _logger.LogWarning("Password reset failed: missing required input.");
            return Result.Failure(ApplicationErrors.Auth.InvalidToken);
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || user.IsDeleted)
        {
            _logger.LogWarning("Password reset failed: no active user for {Email}.", email);
            return Result.Failure(ApplicationErrors.Auth.InvalidToken);
        }

        var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!resetResult.Succeeded)
        {
            _logger.LogWarning(
                "Password reset failed for {Email}. Errors: {Errors}",
                email,
                string.Join("; ", resetResult.Errors.Select(e => $"{e.Code}: {e.Description}")));

            return Result.Failure(resetResult.Errors
                .Select(e => Error.Validation($"Identity.{e.Code}", e.Description))
                .ToList());
        }

        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
        }
        
        var revokeResult = await _refreshToken.RevokeAllForUserAsync(user.Id, ct);
        if (!revokeResult.IsSuccess)
        {
            _logger.LogError(
                "Password reset for user {UserId}, but refresh token revocation failed. Errors: {Errors}",
                user.Id,
                string.Join("; ", revokeResult.Errors.Select(e => e.Code)));
        }

        _logger.LogInformation("Password reset completed for user {UserId}.", user.Id);
        return Result.Success();
    }

    public async Task<bool> RoleExistsAsync(string role, CancellationToken cancellationToken)
    {
        return await _roleManager.RoleExistsAsync(role);
    }

    public async Task<Result> SendEmailConfirmationAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.IsDeleted || user.EmailConfirmed)
        {
            _logger.LogWarning("Email confirmation failed: user {Email} no action taken.", email);
            return Result.Success();
        }


        return await SendTokenEmailAsync(user,
        _userManager.GenerateEmailConfirmationTokenAsync,
        _resendOptions.Value.ConfirmationUrlFormat,
        "Confirm your email",
        "<p>Confirm your email by clicking the link below:</p>",
        "Confirm email",
        "<p>If you didn't create an account, you can ignore this email.</p>",
        cancellationToken);
    }

    public async Task<Result> SendInviteAsync(string email, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || user.IsDeleted) return Result.Success();

        return await SendTokenEmailAsync(user,
            _userManager.GeneratePasswordResetTokenAsync,
            _resendOptions.Value.PasswordResetUrlFormat,   // the "set password" page is the same page
            "You've been invited",
            "<p>An account was created for you. Set your password using the link below:</p>",
            "Set password",
            "<p>If you weren't expecting this, you can ignore this email.</p>",
            ct);
    }

    public async Task<Result> UpdateUserAsync(UpdateUserRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
        {
            _logger.LogWarning("Update current user failed: no authenticated user.");
            return Result.Failure(ApplicationErrors.Authorization.Forbidden);
        }

        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user is null || user.IsDeleted)
        {
            _logger.LogWarning("Update user failed: user {UserId} not found or inactive.", userId);
            return Result.Failure(ApplicationErrors.Auth.UserNotFound);
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? null
            : request.PhoneNumber.Trim();

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            _logger.LogWarning(
                "Failed to update current user {UserId}. Errors: {Errors}",
                userId,
                string.Join("; ", updateResult.Errors.Select(e => $"{e.Code}: {e.Description}")));

            return Result.Failure(updateResult.Errors.Select(e => Error.Create(1, e.Code, e.Description)).ToList());
        }

        _logger.LogInformation("Current user {UserId} profile updated.", userId);
        return Result.Success();
    }

     private async Task<Result> SendTokenEmailAsync(
    AppUser user,
    Func<AppUser, Task<string>> generateToken,
    string urlFormat,
    string subject,
    string introHtml,
    string linkLabel,
    string footerHtml,
    CancellationToken ct)
    {
        var emailVo = EmailAddress.Create(user.Email!);
        if (emailVo.IsError)
        {
            _logger.LogError("Stored email of user {UserId} is invalid.", user.Id);
            return Result.Failure(emailVo.Errors);
        }

        try
        {
            var token = await generateToken(user);

            var link = urlFormat
                .Replace("{email}", Uri.EscapeDataString(user.Email!))
                .Replace("{token}", Uri.EscapeDataString(token));

            var body = $"""
            {introHtml}
            <p><a href="{link}">{linkLabel}</a></p>
            <p>If the link doesn't work, copy this URL into your browser:</p>
            <p style="word-break:break-all">{link}</p>
            {footerHtml}
            """;

            await _emailSender.SendAsync(emailVo.Value, subject, body, ct);
            return Result.Success();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to send '{Subject}' email to user {UserId}.", subject, user.Id);
            return Result.Failure(Error.Failure("Email.SendFailed", "The email could not be sent."));
        }
    }
}