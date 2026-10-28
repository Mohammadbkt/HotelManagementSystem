using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Infrastructure.Identity.Extensions;

public static class AppUserMappingExtensions
{
    public static async Task<AppUserDto> ToDtoAsync(
        this AppUser user,
        UserManager<AppUser> userManager,
        IAppDbContext context,
        CancellationToken cancellationToken)
    {
        var roles = await userManager.GetRolesAsync(user);

        var hotelId = await context.Employees
            .Where(e => e.UserId == user.Id)
            .Select(e => (Guid?)e.HotelId)
            .FirstOrDefaultAsync(cancellationToken);

        return new AppUserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email ?? string.Empty,
            user.UserName ?? user.Email ?? string.Empty,
            hotelId,
            user.PhoneNumber,
            !user.IsDeleted,
            roles.ToList());
    }
}