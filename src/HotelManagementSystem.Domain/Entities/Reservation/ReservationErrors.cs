using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Reservations
{
    public static class ReservationErrors
    {

        public static Error InvalidPricing =>
            Error.Validation(
                "Reservation.InvalidPricing", 
                "Price and total must be greater than zero.");
        public static Error InvalidGuestId =>
            Error.Validation(
                "Reservation.InvalidGuestId",
                "Invalid guest ID.");
        public static Error CannotAssignRoom =>
            Error.Validation(
                "Reservation.CannotAssignRoom",
                "Cannot assign room.");

        public static Error CancellationReasonTooLong =>
            Error.Validation(
                "Reservation.CancellationReasonTooLong",
                "Cancellation reason too long.");

        public static Error SpecialRequestsTooLong =>
            Error.Validation(
                "Reservation.SpecialRequestsTooLong",
                "Special requests too long.");
        public static Error InvalidGuestCount =>
            Error.Validation(
                "Reservation.InvalidGuestCount",
                "Invalid guest count.");
        public static Error InvalidHotelId =>
            Error.Validation(
                "Reservation.InvalidHotelId",
                "Invalid hotel ID.");
        public static Error InvalidRoomId =>
        Error.Validation(
            "Reservation.InvalidRoomId",
            "Invalid room ID.");
        public static Error InvalidRoomCategoryId =>
        Error.Validation(
            "Reservation.InvalidRoomCategoryId",
            "Invalid room Category ID.");

        public static Error InvalidCheckInOutDates =>
            Error.Validation(
                "Reservation.InvalidCheckInOutDates",
                "Invalid check-in and check-out dates.");
        public static Error InvalidReservationNumber =>
            Error.Validation(
                "Reservation.InvalidReservationNumber",
                "Invalid reservation number.");

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

        public static readonly Error CannotCompleteBeforeCheckIn = Error.Validation(
             "Reservation.CannotCompleteBeforeCheckIn",
             "Cannot complete a reservation before Check in.");


        public static readonly Error CannotMarkNoShow = Error.Validation(
            "Reservation.CannotMarkNoShow",
            "Can not mark reservation as no show if the status is not confirmed."
        );



        public static readonly Error CannotCancelReservation = Error.Validation(
        "Reservation.CannotCancelReservation",
        "Only pending or confirmed reservations can be cancelled.");

    }
}