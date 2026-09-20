using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CancelReservation
{
    public sealed class CancelReservationCommandHandler(IAppDbContext context) : IRequestHandler<CancelReservationCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations.FindAsync([request.ReservationId], cancellationToken);
            if(reservation is null)
                return Result.Failure(Error.NotFound("Reservation.NotFound", "Reservation not found."));

            var result =  reservation.Cancel();

            if(result.IsError)
                return Result.Failure(result.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}