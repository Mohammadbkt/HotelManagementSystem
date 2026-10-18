using HotelManagementSystem.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;

namespace HotelManagementSystem.Infrastructure.Email
{
    public sealed class ResendEmailSender(
        IResend resend,
        IOptions<ResendOptions> options,
        ILogger<ResendEmailSender> logger) : IEmailSender
    {
        private readonly IOptions<ResendOptions> _options = options;
        private readonly IResend _resend = resend;
        private readonly ILogger<ResendEmailSender> _logger = logger;

        public async Task SendAsync(Domain.ValueObjects.Email.EmailAddress recipient, string subject, string body, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(subject);

            var response = await _resend.EmailSendAsync(new EmailMessage()
            {
                From = _options.Value.FromEmail,
                To = recipient.Value,
                Subject = subject,
                TextBody = body
            }, cancellationToken);

            if (!response.Success)
            {
                _logger.LogError("Resend send failed for {Recipient}: {Error}",
                    recipient.Value, response.Exception?.Message);
                throw new InvalidOperationException(
                    $"Email send failed: {response.Exception?.Message}");
            }

            _logger.LogInformation("Email {Id} sent to {Recipient}",
                response.Content, recipient.Value);
        }
    }
}