using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Identity.Commands.RefreshToken
{
    public sealed record RefreshTokenCommand(
    string RefreshToken
) : IRequest<Result<AuthResponseDto>>;
}