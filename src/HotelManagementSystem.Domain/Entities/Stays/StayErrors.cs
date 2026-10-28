using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Stays
{
    public static class StayErrors
    {
        public static Error InvalidGuestId =>
            Error.Validation(
                "Stay.InvalidGuestId",
                "Invalid guest ID.");
        public static Error InvalidHotelId =>
            Error.Validation(
                "Stay.InvalidHotelId",
                "Invalid hotel ID.");

        public static Error InvalidRoomId =>
            Error.Validation(
                "Stay.InvalidRoomId",
                "Invalid room ID.");

        public static Error AlreadyCheckedOut =>
            Error.Validation(
                "Stay.AlreadyCheckedOut",
                "The stay has already been checked out.");

        public static Error InvalidCheckOutTime =>
            Error.Validation(
                "Stay.InvalidCheckOutTime",
                "The check-out time cannot be earlier than the check-in time.");

        public static Error InvalidCheckInDate =>
            Error.Validation(
                "Stay.InvalidCheckInDate",
                "The check-in date is invalid.");

        public static Error InvalidReservationId =>
            Error.Validation(
                "Stay.InvalidReservationId",
                "Invalid reservation ID.");

        public static Error NotCheckedOutYet =>
            Error.Validation(
                "Stay.NotCheckedOutYet",
                "Cannot generate an invoice for a stay that has not been checked out.");
        
        public static Error InvalidReservationStatus =>
            Error.Validation(
                "Stay.InvalidReservationStatus",
                "Cannot create a stay from a reservation that is not confirmed.");
    }
}
