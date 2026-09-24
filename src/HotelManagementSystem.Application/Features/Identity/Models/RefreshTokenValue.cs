namespace HotelManagementSystem.Application.Features.Identity.Models;

public sealed record RefreshTokenValue(
    string Value,
    DateTimeOffset ExpiresAt);