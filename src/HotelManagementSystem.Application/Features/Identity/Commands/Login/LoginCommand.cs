using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Identity.Commands.Login
{
    public sealed record LoginCommand(
        string Identifier,
        string Password,
        string? DeviceName,
        string? IpAddress
    ) : IRequest<Result<AuthResponseDto>>;
}