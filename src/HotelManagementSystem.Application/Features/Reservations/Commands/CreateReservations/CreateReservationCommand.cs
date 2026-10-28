using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.CreateReservations
{
    public sealed record CreateReservationsCommand(
        Guid HotelId,
        Guid RoomCategoryId,
        Guid? RoomId,
        DateOnly CheckInDate,
        DateOnly CheckOutDate,
        int NumberOfAdults,
        int NumberOfChildren,
        string? SpecialRequests,
        Guid? GuestId = null
    ) : IRequest<Result<Guid>>;
}