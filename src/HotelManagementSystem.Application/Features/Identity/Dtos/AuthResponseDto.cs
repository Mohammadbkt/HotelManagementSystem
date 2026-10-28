namespace HotelManagementSystem.Application.Features.Identity.Dtos
{
    public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresOnUtc,
    DateTimeOffset RefreshTokenExpiresOnUtc,
    AppUserDto User
    );
}