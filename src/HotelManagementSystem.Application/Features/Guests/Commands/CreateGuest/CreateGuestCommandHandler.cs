using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;

namespace HotelManagementSystem.Application.Features.Guests.Commands.CreateGuest
{
    public sealed class CreateGuestCommandHandler(IAppDbContext context) : IRequestHandler<CreateGuestCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<Guid>> Handle(CreateGuestCommand request, CancellationToken cancellationToken)
        {
            EmailAddress? email = null;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailResult = EmailAddress.Create(request.Email);
                if (emailResult.IsError)
                    return Result<Guid>.Failure(Error.Validation());

                email = emailResult.Value;

            }
            
            PhoneNumber? phone = null;
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var phoneResult = PhoneNumber.Create(request.Phone);
                if (phoneResult.IsError)
                    return Result<Guid>.Failure(Error.Validation());

                phone = phoneResult.Value;
            }

            var guest = Guest.Create(
                            request.FirstName,
                            request.LastName,
                            email,
                            phone,
                            request.DateOfBith);

            if (guest.IsError)
                return Result<Guid>.Failure(guest.Errors);

            _context.Guests.Add(guest.Value);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(guest.Value.Id);

        }
    }
}