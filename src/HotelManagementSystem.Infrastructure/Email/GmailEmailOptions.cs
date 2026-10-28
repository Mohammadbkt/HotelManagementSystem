namespace HotelManagementSystem.Infrastructure.Email;

public sealed class GmailEmailOptions
{
    public const string SectionName = "GmailEmail";

    public string Host { get; init; } = "smtp.gmail.com";
    public int Port { get; init; } = 587;
    public string FromEmail { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string AppPassword { get; init; } = string.Empty;
}