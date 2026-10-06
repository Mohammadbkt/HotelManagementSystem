using System.Buffers.Text;
using System.Security.Cryptography;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Identity;
using HotelManagementSystem.Infrastructure.Data;
using Microsoft.Extensions.Options;

namespace HotelManagementSystem.Infrastructure.Identity
{
    public sealed class RefreshTokenService(IOptions<JwtOptions> options, IAppDbContext context) : IRefreshTokenService
    {
        private readonly IOptions<JwtOptions> _options = options;
        private readonly IAppDbContext _context = context;

        public async Task<Result<RefreshToken>> CreateAsync(Guid userId, string? deviceName, string? ipAddress, CancellationToken cancellationToken)
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

            await _context.RefreshTokens.AddAsync(result.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<RefreshToken>.Success(result.Value);
        }
    }
}