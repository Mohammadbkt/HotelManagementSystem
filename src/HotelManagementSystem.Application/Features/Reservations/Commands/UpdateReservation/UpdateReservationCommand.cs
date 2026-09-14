using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.UpdateReservation
{
    public sealed record UpdateReservationCommand(
        Guid ReservationId,
        Guid RoomId,
        DateOnly CheckInDate,
        DateOnly CheckOutDate
    ) : IRequest<Result>;
}