using HotelManagementSystem.Application.Features.Identity.Models;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<Result<RefreshTokenValue>> CreateAsync(Guid id, CancellationToken cancellationToken);

   }
