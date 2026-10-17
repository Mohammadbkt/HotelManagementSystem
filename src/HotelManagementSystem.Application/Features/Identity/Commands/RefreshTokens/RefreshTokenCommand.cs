using HotelManagementSystem.Application.Features.Identity.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Identity.Commands.RefreshTokens;

    public sealed record RefreshTokenCommand(
    string RefreshToken,
    string DeviceName,
    string IpAddress
    ) : IRequest<Result<AuthResponseDto>>;
