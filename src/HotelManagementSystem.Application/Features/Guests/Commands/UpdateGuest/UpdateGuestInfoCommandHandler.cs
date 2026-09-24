using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuest;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuestInfo
{
    public sealed class UpdateGuestInfoCommandHandler(IAppDbContext context, ILogger<UpdateGuestInfoCommandHandler> logger) : IRequestHandler<UpdateGuestInfoCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateGuestInfoCommandHandler> _logger = logger;

        public async Task<Result> Handle(
            UpdateGuestInfoCommand request,
            CancellationToken cancellationToken)
        {

            var guest = await _context.Guests.FindAsync([request.GuestId], cancellationToken);

            if (guest is null)
            {
                _logger.LogWarning("Guest {GuestId} not found", request.GuestId);
                return Result.Failure(ApplicationErrors.GuestErrors.NotFound);
            }

            var updateResult = guest.UpdateInfo(
                request.FirstName,
                request.LastName,
                request.DateOfBirth);

            if (updateResult.IsError)
            {
                _logger.LogWarning("Failed to update guest {GuestId}: {Errors}", request.GuestId, updateResult.Errors);
                return updateResult;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Guest {GuestId} updated", request.GuestId);
            return Result.Success();
        }
    }
}