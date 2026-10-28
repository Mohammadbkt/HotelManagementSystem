namespace HotelManagementSystem.Infrastructure.Email;

public sealed class ResendOptions
{
    public const string SectionName = "Resend";

    public string ApiKey { get; init; } = string.Empty;

    public string FromEmail { get; init; } = "onboarding@resend.dev";

    public string ConfirmationUrlFormat { get; init; } = string.Empty;
    
    public string PasswordResetUrlFormat { get; init; } = string.Empty;
    
}