using HotelManagementSystem.Application.Features.Reservations.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Queries.GetReservationById
{
    public sealed record GetReservationByIdQuery(
        Guid ReservationId
    ) : IRequest<Result<ReservationDto>>;

  
}