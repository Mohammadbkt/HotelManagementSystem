using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuest;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuestInfo
{
    public sealed class UpdateGuestInfoCommandHandler(IAppDbContext context) : IRequestHandler<UpdateGuestInfoCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(
            UpdateGuestInfoCommand request,
            CancellationToken cancellationToken)
        {
            var guest = await _context.Guests.FindAsync([request.GuestId], cancellationToken);

            if (guest is null)
                return Result.Failure(ApplicationErrors.Guest.NotFound);

            var updateResult = guest.UpdateInfo(
                request.FirstName,
                request.LastName,
                request.DateOfBirth);

            if (updateResult.IsError)
                return updateResult;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}