using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Application.Features.Identity.Models;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Application.Common.Interfaces
{
    public interface IAccessTokenGenerator
    {
        Result<AccessToken> Generate(AppUserDto user);
    }
}