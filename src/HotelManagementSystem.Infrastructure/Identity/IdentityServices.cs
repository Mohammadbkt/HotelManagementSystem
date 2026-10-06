using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Employee;
using HotelManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Infrastructure.Identity;

public sealed class IdentityServices(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IAppDbContext context,
    ILogger<IdentityServices> logger) : IIdentityService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
    private readonly IAppDbContext _context = context;
    private readonly ILogger<IdentityServices> _logger = logger;

    public async Task<Result<AppUserDto>> AuthenticateAsync(string identifier, string password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(identifier)
                   ?? await _userManager.FindByNameAsync(identifier)
                   ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == identifier);

        if (user is null)
        {
            _logger.LogWarning(
                "Login failed: user not found for identifier '{Identifier}'.",
                identifier);

            return Result<AppUserDto>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid credentials."));
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            _logger.LogWarning(
                "Login failed: account locked for user '{UserId}' ({Email}).",
                user.Id, user.Email);

            return Result<AppUserDto>.Failure(
                Error.Forbidden("Auth.LockedOut", "This account is temporarily locked. Try again later."));
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            await _userManager.AccessFailedAsync(user);

            _logger.LogWarning(
                "Login failed: invalid password for user '{UserId}' ({Email}).",
                user.Id, user.Email);

            return Result<AppUserDto>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid credentials."));
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = await _userManager.GetRolesAsync(user);


        _logger.LogInformation(
            "User '{Email}' authenticated successfully.",
            user.Email);

        var userDto = await ToDtoAsync(user, cancellationToken);
        return Result<AppUserDto>.Success(userDto);
    }

    public async Task<Result<AppUserDto>> CreateUserAsync(string email, string username, string password, string firstName, string lastName, string? phoneNumber, string role, CancellationToken cancellationToken)
    {
        var roleExists = await _roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            _logger.LogWarning(
                "Role '{Role}' does not exist when creating user '{Email}'.",
                role, email);

            return Result<AppUserDto>.Failure(
                Error.Failure("Auth.RoleNotFound", $"Role '{role}' does not exist."));
        }

        var user = new AppUser
        {
            UserName = username,
            Email = email,
            PhoneNumber = phoneNumber,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = false
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            _logger.LogWarning(
                "User creation failed for email '{Email}'. Errors: {Errors}.",
                email,
                string.Join("; ", createResult.Errors.Select(e => $"{e.Code}: {e.Description}")));

            var errors = createResult.Errors
                .Select(e => Error.Validation(e.Code, e.Description))
                .ToList();

            return Result<AppUserDto>.Failure(errors);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            _logger.LogWarning(
                "Adding role '{Role}' to user '{UserId}' failed. Errors: {Errors}. Rolling back user creation.",
                role,
                user.Id,
                string.Join("; ", roleResult.Errors.Select(e => $"{e.Code}: {e.Description}")));

            var errors = roleResult.Errors
                .Select(e => Error.Validation(e.Code, e.Description))
                .ToList();

            await _userManager.DeleteAsync(user);

            return Result<AppUserDto>.Failure(errors);
        }


        _logger.LogInformation(
            "User '{Email}' created successfully with role '{Role}'.",
            email, role);

        var userDto = await ToDtoAsync(user, cancellationToken);
        return Result<AppUserDto>.Success(userDto);
    }

    public async Task<Result<AppUserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            _logger.LogWarning("User not found for Id '{UserId}'.", userId);
            return Result<AppUserDto>.Failure(
                Error.NotFound("Auth.UserNotFound", $"User with ID '{userId}' not found."));
        }

        var roles = await _userManager.GetRolesAsync(user);

        var userDto = await ToDtoAsync(user, cancellationToken);
        return Result<AppUserDto>.Success(userDto);
    }

    public async Task<string?> GetUserNameAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            _logger.LogWarning("User not found for Id '{UserId}' when fetching username.", userId);
            return null;
        }

        return user.UserName;
    }

    public async Task<bool> IsInRoleAsync(Guid userId, EmployeeRole role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            _logger.LogWarning("User not found for Id '{UserId}' when checking role.", userId);
            return false;
        }

        var roleName = role.ToString();
        var isInRole = await _userManager.IsInRoleAsync(user, roleName);

        return isInRole;
    }

    private async Task<AppUserDto> ToDtoAsync(AppUser user, CancellationToken ct)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var hotelId = await _context.Employees
            .Where(e => e.UserId == user.Id)
            .Select(e => (Guid?)e.HotelId)
            .FirstOrDefaultAsync(ct);

        return new AppUserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email ?? string.Empty,
            user.UserName ?? user.Email ?? string.Empty,
            hotelId,
            user.PhoneNumber,
            roles.ToList(),
            Array.Empty<string>());
    }
}