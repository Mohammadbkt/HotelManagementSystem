using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Identity;

namespace HotelManagementSystem.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    Result<RefreshToken> Create(Guid userId, string? deviceName, string? ipAddress);

}
