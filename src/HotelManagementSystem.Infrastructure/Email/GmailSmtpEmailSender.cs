using HotelManagementSystem.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HotelManagementSystem.Infrastructure.Email;

public sealed class GmailSmtpEmailSender(
    IOptions<GmailEmailOptions> options,
    ILogger<GmailSmtpEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(
        Domain.ValueObjects.Email.EmailAddress recipient,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        var config = options.Value;

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(config.FromEmail));
        message.To.Add(MailboxAddress.Parse(recipient.Value));
        message.Subject = subject;
        message.Body = new TextPart("html")
        {
            Text = body
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            config.Host,
            config.Port,
            SecureSocketOptions.StartTls,
            cancellationToken);

        await client.AuthenticateAsync(
            config.Username,
            config.AppPassword,
            cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        logger.LogInformation(
            "Email sent to {Recipient} via Gmail SMTP",
            recipient.Value);
    }
}
