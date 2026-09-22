using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.ConfirmReservation
{
    public sealed class ConfirmReservationCommandHandler(IAppDbContext context, ILogger<ConfirmReservationCommandHandler> logger) : IRequestHandler<ConfirmReservationCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<ConfirmReservationCommandHandler> _logger = logger;

        public async Task<Result> Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations.FindAsync([request.ReservationId], cancellationToken);
            if (reservation is null)
            {
                _logger.LogWarning("Reservation {ReservationId} not found", request.ReservationId);
                return Result.Failure(ApplicationErrors.Reservation.NotFound);
            }

            var result = reservation.Confirm();

            if (result.IsError)
            {
                _logger.LogWarning("Failed to confirm reservation {ReservationId}: {Errors}", request.ReservationId, result.Errors);
                return Result.Failure(result.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reservation {ReservationId} confirmed", request.ReservationId);
            return Result.Success();

        }
    }
}