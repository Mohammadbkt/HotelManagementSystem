using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Identity.Commands.RegisterGuest
{
    public sealed record RegisterGuestCommand(
        string Email,
        string Username,
        string Password,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        string? DeviceName,
        string? IpAddress,
        string? PhoneNumber = null
    ) : IRequest<Result<RegisterGuestResponseDto>>;
}