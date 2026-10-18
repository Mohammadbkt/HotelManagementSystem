using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Infrastructure.Data;
using HotelManagementSystem.Infrastructure.Email;
using HotelManagementSystem.Infrastructure.Identity;
using HotelManagementSystem.Infrastructure.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Resend;

namespace HotelManagementSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(o => Encoding.UTF8.GetBytes(o.Secret).Length >= 32, "JWT secret must be at least 32 bytes")
            .ValidateOnStart();

        services.AddHttpContextAccessor();
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
        });

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AppDbContext>();

       

        services.AddScoped<ICurrentUserService, CurrentUser>();
        services.AddScoped<IIdentityService, IdentityServices>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAccessTokenGenerator, JwtService>();
        services.AddScoped<IEmailSender, ResendEmailSender>();

        // Registers the scheme only; the settings come from the Configure<> block below.
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwt) =>
            {
                var o = jwt.Value;

                bearer.MapInboundClaims = false;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(o.Secret)),

                    ValidateIssuer = true,
                    ValidIssuer = o.Issuer,

                    ValidateAudience = true,
                    ValidAudience = o.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    NameClaimType = JwtRegisteredClaimNames.UniqueName,
                    RoleClaimType = ClaimTypes.Role
                };
            });


        services.AddOptions<ResendOptions>()
            .BindConfiguration(ResendOptions.SectionName)
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ApiKey),
                "Resend API key is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.FromEmail),
                "Resend sender email is required.")
            .ValidateOnStart();

        services.AddResend(
            configuration[$"{ResendOptions.SectionName}:ApiKey"] ?? string.Empty);

        services.AddScoped<AppDbContextInitializer>();

        return services;
    }
}