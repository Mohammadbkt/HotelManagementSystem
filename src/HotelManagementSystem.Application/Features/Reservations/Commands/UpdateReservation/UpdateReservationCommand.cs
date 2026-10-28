using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Reservations.Commands.UpdateReservation
{
    public sealed record UpdateReservationCommand(
        Guid ReservationId,
        Guid RoomCategoryId,
        DateOnly CheckInDate,
        DateOnly CheckOutDate,
         int NumberOfAdults,
       int NumberOfChildren,
       string? SpecialRequests
    ) : IRequest<Result>;
}