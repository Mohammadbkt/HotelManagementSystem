using HotelManagementSystem.Application.Features.Identity.Models;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Identity;

namespace HotelManagementSystem.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<Result<RefreshToken>> CreateAsync(Guid userId, string? deviceName, string? ipAddress, CancellationToken cancellationToken);

}
