using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Reservations
{
    public static class ReservationErrors
    {
        public static Error InvalidGuestId =>
            Error.Validation(
                "Reservation.InvalidGuestId",
                "Invalid guest ID.");
        public static Error InvalidRoomId =>
        Error.Validation(
            "Reservation.InvalidRoomId",
            "Invalid room ID.");
        public static Error InvalidCheckInOutDates =>
            Error.Validation(
                "Reservation.InvalidCheckInOutDates",
                "Invalid check-in and check-out dates.");

        public static Error ReservationAlreadyCancelled =>
            Error.Validation(
                "Reservation.AlreadyCancelled",
                "Reservation is already cancelled.");

        public static Error CannotUpdateReservation =>
            Error.Validation(
                "Reservation.CannotUpdateReservation",
                "Reservation is not pending.");

        public static Error CannotConfirmReservation =>
            Error.Validation(
                "Reservation.CannotConfirmReservation",
                "Reservation is not pending.");

        public static Error CannotCheckIn =>
        Error.Validation(
            "Reservation.CannotCheckIn",
            "Reservation is not confirmed.");

        public static readonly Error RoomAlreadyBooked = Error.Validation(
            "Reservation.RoomAlreadyBooked",
            "The room is already booked for an overlapping date range.");

        public static readonly Error CannotCancelAfterCheckIn = Error.Validation(
            "Reservation.CannotCancelAfterCheckIn",
            "Cannot cancel a reservation after Check in.");



    }
}