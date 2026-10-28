using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Entities.Reservations;

namespace HotelManagementSystem.Application.Features.Reservations.Dtos
{
    public sealed record ReservationDto(
        Guid ReservationId,
        Guid HotelId,
        Guid RoomId,
        ReservationStatus ReservationStatus,
        DateOnly CheckInDate,
        DateOnly CheckOutDate
    );
}