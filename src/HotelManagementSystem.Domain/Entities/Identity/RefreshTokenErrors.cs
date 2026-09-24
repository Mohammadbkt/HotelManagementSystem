using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Identity;

public static class RefreshTokenErrors
{
    public static Error TokenRequired =>
        Error.Validation(
            "RefreshToken.Token",
            "Refresh token is required.");

    public static Error UserIdRequired =>
        Error.Validation(
            "RefreshToken.UserId",
            "User id is required.");


    public static Error InvalidExpirationDate =>
        Error.Validation(
            "RefreshToken.ExpiredToken",
            "Expired Token.");

    public static Error AlreadyRevoked =>
        Error.Conflict(
            "RefreshToken.AlreadyRevoked",
            "Token Already Revoked.");
    
    public static Error InvalidRevocationDate =>
        Error.Validation(
            "RefreshToken.InvalidRevocationDate",
            "Invalid Revocation Date.");

    public static Error AlreadyExpired =>
        Error.Validation(
            "RefreshToken.AlreadyExpired",
            "Token Already Expired.");
    public static Error InvalidReplacedByToken =>
        Error.Validation(
            "RefreshToken.InvalidReplacedByToken",
            "Invalid ReplacedBy Token.");
}
