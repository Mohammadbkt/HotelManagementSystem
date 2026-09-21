using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Reservations.Dtos;
using HotelManagementSystem.Application.Features.Reservations.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Reservations.Queries.GetReservationById
{
    public sealed class GetReservationByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetReservationByIdQuery, Result<ReservationDto>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<ReservationDto>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
        {
            var reservationDto = await _context.Reservations
                                    .AsNoTracking()
                                    .Where(r => r.Id == request.ReservationId)
                                    .ToReservationDto()
                                    .FirstOrDefaultAsync(cancellationToken);

            if(reservationDto is null)
                return Result<ReservationDto>.Failure(ApplicationErrors.Reservation.NotFound);

            return Result<ReservationDto>.Success(reservationDto);
        }
    }
}