using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CancelReservation
{
    public sealed record CancelReservationCommand(
        Guid ReservationId
    ) : IRequest<Result>;
}