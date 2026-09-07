using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Reservations;

namespace HotelManagementSystem.Domain.Services;

public static class ReservationAvailabilityService
{
    public static Result EnsureNoOverlap(
        IEnumerable<Reservation> existingReservationsForRoom,
        DateOnly checkInDate,
        DateOnly checkOutDate)
    {
        bool hasOverlap = existingReservationsForRoom.Any(r =>
            IsActive(r.Status)
            && checkInDate < r.CheckOutDate
            && r.CheckInDate < checkOutDate);

        return hasOverlap
            ? ReservationErrors.RoomAlreadyBooked
            : Result.Success();
    }

    private static bool IsActive(ReservationStatus status)
        => status is ReservationStatus.Pending or ReservationStatus.Confirmed;
}