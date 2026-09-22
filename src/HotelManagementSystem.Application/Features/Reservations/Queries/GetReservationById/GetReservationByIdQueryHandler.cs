using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Reservations.Dtos;
using HotelManagementSystem.Application.Features.Reservations.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Reservations.Queries.GetReservationById
{
    public sealed class GetReservationByIdQueryHandler(IAppDbContext context, ILogger<GetReservationByIdQueryHandler> logger) : IRequestHandler<GetReservationByIdQuery, Result<ReservationDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetReservationByIdQueryHandler> _logger = logger;

        public async Task<Result<ReservationDto>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
        {
            var reservationDto = await _context.Reservations
                                    .AsNoTracking()
                                    .Where(r => r.Id == request.ReservationId)
                                    .ToReservationDto()
                                    .FirstOrDefaultAsync(cancellationToken);

            if(reservationDto is null)
            {
                _logger.LogWarning("Reservation {ReservationId} not found", request.ReservationId);
                return Result<ReservationDto>.Failure(ApplicationErrors.Reservation.NotFound);
            }

            _logger.LogInformation("Reservation {ReservationId} retrieved", request.ReservationId);
            return Result<ReservationDto>.Success(reservationDto);
        }
    }
}