using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Guests.Commands.CreateGuest
{
    public sealed class CreateGuestCommandHandler(IAppDbContext context, ILogger<CreateGuestCommandHandler> logger) : IRequestHandler<CreateGuestCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<CreateGuestCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(CreateGuestCommand request, CancellationToken cancellationToken)
        {

            EmailAddress? email = null;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailResult = EmailAddress.Create(request.Email);
                if (emailResult.IsError)
                {
                    _logger.LogWarning("Invalid guest email {Email}: {Errors}", request.Email, emailResult.Errors);
                    return Result<Guid>.Failure(emailResult.Errors);
                }

                email = emailResult.Value;
            }

            PhoneNumber? phone = null;
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var phoneResult = PhoneNumber.Create(request.Phone);
                if (phoneResult.IsError)
                {
                    _logger.LogWarning("Invalid guest phone {Phone}: {Errors}", request.Phone, phoneResult.Errors);
                    return Result<Guid>.Failure(phoneResult.Errors);
                }

                phone = phoneResult.Value;
            }

            if (email is not null || phone is not null)
            {
                var duplicateExists = await _context.Guests.AnyAsync(g =>
                    (email != null && g.Email != null && g.Email.Value.Value == email.Value.Value) ||
                    (phone != null && g.PhoneNumber != null && g.PhoneNumber.Value.Value == phone.Value.Value),
                    cancellationToken);

                if (duplicateExists)
                {
                    _logger.LogWarning("Guest already exists (email {Email}, phone {Phone})", request.Email, request.Phone);
                    return Result<Guid>.Failure(ApplicationErrors.Guest.AlreadyExists);
                }
            }

            var guest = Guest.Create(
                            request.FirstName,
                            request.LastName,
                            email,
                            phone,
                            request.DateOfBirth);

            if (guest.IsError)
            {
                _logger.LogWarning("Failed to create guest: {Errors}", guest.Errors);
                return Result<Guid>.Failure(guest.Errors);
            }

            _context.Guests.Add(guest.Value);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Guest {GuestId} created", guest.Value.Id);
            return Result<Guid>.Success(guest.Value.Id);
        }
    }
}