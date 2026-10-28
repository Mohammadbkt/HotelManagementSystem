namespace HotelManagementSystem.Application.Common.Interfaces;

public sealed record IssuedRefreshToken(
    string RawToken,
    DateTimeOffset ExpiresOnUtc);
