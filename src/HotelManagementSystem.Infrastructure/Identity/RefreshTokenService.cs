using System.Buffers.Text;
using System.Security.Cryptography;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Identity;
using Microsoft.Extensions.Options;

namespace HotelManagementSystem.Infrastructure.Identity
{
    public sealed class RefreshTokenService(IOptions<JwtOptions> options) : IRefreshTokenService
    {
        private readonly IOptions<JwtOptions> _options = options;

        public  Result<RefreshToken> Create(Guid userId, string? deviceName, string? ipAddress)
        {

            var token = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(64));

            var result = RefreshToken.Create(
                token,
                userId,
                DateTimeOffset.UtcNow.AddDays(_options.Value.RefreshTokenDays),
                deviceName,
                ipAddress);

            if(result.IsError)
            {
                return Result<RefreshToken>.Failure(result.Errors);
            }

            return Result<RefreshToken>.Success(result.Value);
        }
    }
}