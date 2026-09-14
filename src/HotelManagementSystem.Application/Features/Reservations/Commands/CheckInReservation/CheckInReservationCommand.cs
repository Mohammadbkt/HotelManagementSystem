using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CheckInReservation
{
    public sealed record CheckInReservationCommand(
        Guid ReservationId
    ) : IRequest<Result>;
}