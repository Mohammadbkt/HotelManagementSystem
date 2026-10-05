namespace HotelManagementSystem.Application.Features.Identity.Models;

public sealed record AccessToken(
    string Value,
    DateTimeOffset ExpiresAtUtc);