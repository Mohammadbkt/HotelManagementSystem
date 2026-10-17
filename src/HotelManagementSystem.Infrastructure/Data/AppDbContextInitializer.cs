using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Entities.Employee;
using HotelManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Infrastructure.Data;

public sealed class AppDbContextInitializer(
    AppDbContext context,
    RoleManager<IdentityRole<Guid>> roleManager,
    UserManager<AppUser> userManager,
    IConfiguration configuration,
    ILogger<AppDbContextInitializer> logger)
{
    public Task MigrateAsync() => context.Database.MigrateAsync();

    public async Task SeedAsync()
    {
        foreach (var roleName in Roles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}': {string.Join("; ", result.Errors.Select(e => e.Description))}");

            logger.LogInformation("Seeded role {Role}", roleName);
        }

        await SeedManagerAsync();
    }

    private async Task SeedManagerAsync()
    {
        var email = configuration["Seed:ManagerEmail"];
        var password = configuration["Seed:ManagerPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogInformation("No Seed:ManagerEmail/ManagerPassword configured; skipping initial manager.");
            return;
        }

        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = "Initial",
            LastName = "Manager"
        };

        var create = await userManager.CreateAsync(user, password);
        if (!create.Succeeded)
            throw new InvalidOperationException(
                $"Failed to seed manager: {string.Join("; ", create.Errors.Select(e => e.Description))}");

        var addRole = await userManager.AddToRoleAsync(user, nameof(EmployeeRole.Manager));
        if (!addRole.Succeeded)
            throw new InvalidOperationException(
                $"Failed to assign Manager role: {string.Join("; ", addRole.Errors.Select(e => e.Description))}");

        logger.LogInformation("Seeded initial manager {Email}", email);
    }
}

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this IHost app, bool migrate)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();

        if (migrate)
            await initialiser.MigrateAsync();

        await initialiser.SeedAsync();
    }
}