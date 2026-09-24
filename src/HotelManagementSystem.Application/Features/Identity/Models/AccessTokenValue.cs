namespace HotelManagementSystem.Application.Features.Identity.Models;

public sealed record AccessTokenValue(
    string Value,
    DateTimeOffset ExpiresAt);