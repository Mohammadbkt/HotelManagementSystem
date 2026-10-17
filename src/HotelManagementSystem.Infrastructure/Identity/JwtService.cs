using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Application.Features.Identity.Models;
using HotelManagementSystem.Domain.Common.Result;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HotelManagementSystem.Infrastructure.Identity
{
    public sealed class JwtService(IOptions<JwtOptions> options) : IAccessTokenGenerator
    {
        private readonly IOptions<JwtOptions> _options = options;

        public Result<AccessToken> Generate(AppUserDto user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var now = DateTimeOffset.UtcNow;

            var expires = now.AddMinutes(_options.Value.AccessTokenMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)

            };

            if (user.Roles is not null)
            {
                claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            }

            if (user.HotelId.HasValue)
            {
                claims.Add(new Claim("hotelId", user.HotelId.Value.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: _options.Value.Issuer,
                audience: _options.Value.Audience,
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: expires.UtcDateTime,
                signingCredentials: creds
            );

            var handler = new JwtSecurityTokenHandler();

            return Result<AccessToken>.Success(new AccessToken(handler.WriteToken(token), expires));
        }
    }
}