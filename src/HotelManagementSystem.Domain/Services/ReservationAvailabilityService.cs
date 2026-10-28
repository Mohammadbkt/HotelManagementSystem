using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Reservations;

namespace HotelManagementSystem.Domain.Services;

public static class ReservationAvailabilityService
{
    public static Result EnsureAvailability(
    IEnumerable<Reservation> existingReservationsInCategory,
    DateOnly checkInDate,
    DateOnly checkOutDate,
    int roomCountInCategory)
    {
        var overlappingCount = existingReservationsInCategory.Count(r =>
            IsActive(r.Status)
            && checkInDate < r.CheckOutDate
            && r.CheckInDate < checkOutDate);

        return overlappingCount >= roomCountInCategory
            ? Result.Failure(ReservationErrors.RoomAlreadyBooked)
            : Result.Success();
    }

    private static bool IsActive(ReservationStatus status)
        => status is ReservationStatus.Pending
              or ReservationStatus.Confirmed
              or ReservationStatus.CheckedIn;
}