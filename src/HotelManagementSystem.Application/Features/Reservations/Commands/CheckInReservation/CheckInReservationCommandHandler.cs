using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CheckInReservation
{
    public sealed class CheckInReservationCommandHandler(IAppDbContext context) : IRequestHandler<CheckInReservationCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(CheckInReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations.FindAsync([request.ReservationId], cancellationToken);
            if(reservation is null)
                return Result.Failure(Error.NotFound("Reservation.NotFound", "Reservation not found."));

            var result =  reservation.MarkAsCheckedIn();

            if(result.IsError)
                return Result.Failure(result.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}