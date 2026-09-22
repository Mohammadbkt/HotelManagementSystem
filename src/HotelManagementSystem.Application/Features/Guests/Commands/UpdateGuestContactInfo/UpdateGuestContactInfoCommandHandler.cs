using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuestContactInfo
{
    public sealed class UpdateGuestContactInfoCommandHandler(IAppDbContext context, ILogger<UpdateGuestContactInfoCommandHandler> logger)
        : IRequestHandler<UpdateGuestContactInfoCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateGuestContactInfoCommandHandler> _logger = logger;

        public async Task<Result> Handle(
            UpdateGuestContactInfoCommand request,
            CancellationToken cancellationToken)
        {

            var guest = await _context.Guests
                .FirstOrDefaultAsync(g => g.Id == request.GuestId, cancellationToken);

            if (guest is null)
            {
                _logger.LogWarning("Guest {GuestId} not found", request.GuestId);
                return Result.Failure(ApplicationErrors.Guest.NotFound);
            }

            EmailAddress? email = null;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailResult = EmailAddress.Create(request.Email);
                if (emailResult.IsError)
                {
                    _logger.LogWarning("Invalid email {Email} for guest {GuestId}: {Errors}", request.Email, request.GuestId, emailResult.Errors);
                    return Result.Failure(emailResult.Errors);
                }

                email = emailResult.Value;
            }

            PhoneNumber? phone = null;
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var phoneResult = PhoneNumber.Create(request.Phone);
                if (phoneResult.IsError)
                {
                    _logger.LogWarning("Invalid phone {Phone} for guest {GuestId}: {Errors}", request.Phone, request.GuestId, phoneResult.Errors);
                    return Result.Failure(phoneResult.Errors);
                }

                phone = phoneResult.Value;
            }

            var updateResult = guest.UpdateContactInfo(email, phone);
            if (updateResult.IsError)
            {
                _logger.LogWarning("Failed to update contact info for guest {GuestId}: {Errors}", request.GuestId, updateResult.Errors);
                return updateResult;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Guest {GuestId} contact info updated", request.GuestId);
            return Result.Success();
        }
    }
}