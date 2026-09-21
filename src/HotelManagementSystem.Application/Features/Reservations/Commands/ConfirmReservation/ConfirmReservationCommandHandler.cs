using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.ConfirmReservation
{
    public sealed class ConfirmReservationCommandHandler(IAppDbContext context) : IRequestHandler<ConfirmReservationCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations.FindAsync([request.ReservationId], cancellationToken);
            if (reservation is null)
                return Result.Failure(ApplicationErrors.Reservation.NotFound);

            var result = reservation.Confirm();

            if (result.IsError)
                return Result.Failure(result.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}