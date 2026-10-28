
namespace HotelManagementSystem.Application.Features.Identity.Dtos;

public sealed record RegisterGuestResponseDto(
    string Email,
    bool RequiresEmailVerification);