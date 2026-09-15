using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Stays.Commands.CheckInGuest
{
    public sealed record CheckInGuestCommand(
        Guid GuestId,
        Guid RoomId,
        Guid? ReservationId
    ) : IRequest<Result<Guid>>;
}