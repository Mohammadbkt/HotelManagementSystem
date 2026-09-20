using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuestContactInfo
{
    public sealed class UpdateGuestContactInfoCommandHandler(IAppDbContext context)
        : IRequestHandler<UpdateGuestContactInfoCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(
            UpdateGuestContactInfoCommand request,
            CancellationToken cancellationToken)
        {
            var guest = await _context.Guests
                .FirstOrDefaultAsync(g => g.Id == request.GuestId, cancellationToken);

            if (guest is null)
                return Result.Failure(Error.NotFound("Guest.NotFound", "Guest not found."));

            EmailAddress? email = null;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailResult = EmailAddress.Create(request.Email);
                if (emailResult.IsError)
                    return Result.Failure(emailResult.Errors);

                email = emailResult.Value;
            }

            PhoneNumber? phone = null;
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var phoneResult = PhoneNumber.Create(request.Phone);
                if (phoneResult.IsError)
                    return Result.Failure(phoneResult.Errors);

                phone = phoneResult.Value;
            }

            var updateResult = guest.UpdateContactInfo(email, phone);
            if (updateResult.IsError)
                return updateResult;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}