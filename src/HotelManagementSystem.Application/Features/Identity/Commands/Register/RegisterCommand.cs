using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Identity.Commands.Register
{
    public sealed record RegisterCommand(
        string Email,
        string Username,
        string Password,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        string? PhoneNumber = null
    ) : IRequest<Result<AuthResponseDto>>;
}