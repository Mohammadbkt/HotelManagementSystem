using HotelManagementSystem.Domain.ValueObjects.Email;

namespace HotelManagementSystem.Application.Common.Interfaces
{
    public interface IEmailSender
    {
        public Task SendAsync(EmailAddress recipient, string subject, string body, CancellationToken cancellationToken);
    }
}