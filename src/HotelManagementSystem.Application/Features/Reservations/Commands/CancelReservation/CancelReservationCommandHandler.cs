using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CancelReservation
{
    public sealed class CancelReservationCommandHandler(IAppDbContext context, ILogger<CancelReservationCommandHandler> logger) : IRequestHandler<CancelReservationCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<CancelReservationCommandHandler> _logger = logger;

        public async Task<Result> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations.FindAsync([request.ReservationId], cancellationToken);
            if(reservation is null)
            {
                _logger.LogWarning("Reservation {ReservationId} not found", request.ReservationId);
                return Result.Failure(ApplicationErrors.Reservation.NotFound);
            }

            var result =  reservation.Cancel();

            if(result.IsError)
            {
                _logger.LogWarning("Failed to cancel reservation {ReservationId}: {Errors}", request.ReservationId, result.Errors);
                return Result.Failure(result.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reservation {ReservationId} cancelled", request.ReservationId);
            return Result.Success();
        }
    }
}