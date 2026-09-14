using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CreateReservations
{
    public sealed record CreateReservationsCommand(
        Guid GuestId,
        Guid RoomId,
        DateOnly CheckInDate,
        DateOnly CheckOutDate
    ) : IRequest<Result<Guid>>;
}