using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Application.Common.Errors;

public static class ApplicationErrors
{
    public static class Hotel
    {
        public static readonly Error NotFound =
            Error.NotFound(
                "Hotel.NotFound",
                "Hotel not found.");

        public static readonly Error AlreadyExists =
            Error.Conflict(
                "Hotel.AlreadyExists",
                "A hotel with the same information already exists.");
    }

    public static class RoomCategory
    {
        public static readonly Error NotFound =
            Error.NotFound(
                "RoomCategory.NotFound",
                "Room category not found.");

        public static readonly Error AlreadyExists =
            Error.Conflict(
                "RoomCategory.AlreadyExists",
                "A room category with the same name already exists.");

        public static readonly Error HasRooms =
            Error.Conflict(
                "RoomCategory.HasRooms",
                "The room category cannot be deleted because it has rooms.");

        public static readonly Error HotelMismatch =
            Error.Validation(
                "RoomCategory.HotelMismatch",
                "The selected room category does not belong to this hotel.");
    }


    public static class Room
    {
        public static readonly Error NotFound =
            Error.NotFound(
                "Room.NotFound",
                "Room not found.");

        public static readonly Error AlreadyExists =
            Error.Conflict(
                "Room.AlreadyExists",
                "A room with the same room number already exists.");

        public static readonly Error AlreadyBooked =
            Error.Conflict(
                "Room.AlreadyBooked",
                "The room is already booked for the requested period.");

        public static readonly Error NotAvailable =
            Error.Conflict(
                "Room.NotAvailable",
                "The room is not available for the requested operation.");

        public static readonly Error NotOccupied =
            Error.Conflict(
                "Room.NotOccupied",
                "The room is not currently occupied.");

        public static readonly Error CannotDelete =
            Error.Conflict(
                "Room.CannotDelete",
                "The room cannot be deleted in its current state.");

        public static readonly Error HasActiveReservation =
            Error.Validation(
                "Room.HasActiveReservation",
                "Cannot change category: the room has an active reservation.");

        public static readonly Error HasActiveStay =
            Error.Validation(
                "Room.HasActiveStay",
                "Cannot change category: the room has an active stay.");
    }

    public static class GuestErrors
    {
        public static readonly Error NotFound =
            Error.NotFound(
                "Guest.NotFound",
                "Guest not found.");

        public static readonly Error AlreadyExists =
            Error.Conflict(
                "Guest.AlreadyExists",
                "A guest with the same information already exists.");

        public static readonly Error HasActiveStay =
            Error.Conflict(
                "Guest.HasActiveStay",
                "The guest has an active stay.");
    }

    public static class Employee
    {
        public static readonly Error NotFound =
            Error.NotFound(
                "Employee.NotFound",
                "Employee not found.");

        public static readonly Error AlreadyExists =
            Error.Conflict(
                "Employee.AlreadyExists",
                "An employee with the same information already exists.");

        public static readonly Error CannotDeleteSelf = 
            Error.Forbidden(
                "CannotDeleteSelf", 
                "Cannot Delete Self.");
    }

    public static class Reservation
    {
        public static readonly Error NotFound =
            Error.NotFound(
                "Reservation.NotFound",
                "Reservation not found.");

        public static readonly Error AlreadyExists =
            Error.Conflict(
                "Reservation.AlreadyExists",
                "A reservation with the same information already exists.");

        public static readonly Error RoomAlreadyBooked =
            Error.Conflict(
                "Reservation.RoomAlreadyBooked",
                "The room is already booked for the requested period.");

        public static readonly Error GuestNotFound =
            Error.NotFound(
                "Reservation.GuestNotFound",
                "The guest associated with the reservation was not found.");

        public static readonly Error RoomNotFound =
            Error.NotFound(
                "Reservation.RoomNotFound",
                "The room associated with the reservation was not found.");

        public static readonly Error InvalidState =
            Error.Conflict(
                "Reservation.InvalidState",
                "The reservation is not in a valid state for this operation.");

        public static readonly Error CannotCancel =
            Error.Conflict(
                "Reservation.CannotCancel",
                "The reservation cannot be cancelled in its current state.");

        public static readonly Error CannotCheckIn =
            Error.Conflict(
                "Reservation.CannotCheckIn",
                "The reservation cannot be checked in in its current state.");

        public static readonly Error CannotComplete =
            Error.Conflict(
                "Reservation.CannotComplete",
                "The reservation cannot be completed in its current state.");
    }

    public static class Stay
    {
        public static readonly Error NotFound =
            Error.NotFound(
                "Stay.NotFound",
                "Stay not found.");

        public static readonly Error AlreadyExists =
            Error.Conflict(
                "Stay.AlreadyExists",
                "An active stay already exists for this guest and room.");

        public static readonly Error GuestNotFound =
            Error.NotFound(
                "Stay.GuestNotFound",
                "The guest associated with the stay was not found.");

        public static readonly Error RoomNotFound =
            Error.NotFound(
                "Stay.RoomNotFound",
                "The room associated with the stay was not found.");

        public static readonly Error ReservationNotFound =
            Error.NotFound(
                "Stay.ReservationNotFound",
                "The reservation associated with the stay was not found.");

        public static readonly Error AlreadyCheckedOut =
            Error.Conflict(
                "Stay.AlreadyCheckedOut",
                "The guest has already checked out.");

        public static readonly Error NotCheckedOut =
            Error.Conflict(
                "Stay.NotCheckedOut",
                "The guest has not checked out yet.");

        public static readonly Error InvalidState =
            Error.Conflict(
                "Stay.InvalidState",
                "The stay is not in a valid state for this operation.");
    }

    public static class Invoice
    {
        public static readonly Error NotFound =
            Error.NotFound(
                "Invoice.NotFound",
                "Invoice not found.");

        public static readonly Error AlreadyExistsForStay =
            Error.Conflict(
                "Invoice.AlreadyExistsForStay",
                "An active invoice already exists for this stay.");

        public static readonly Error GuestMismatch =
            Error.Conflict(
                "Invoice.GuestMismatch",
                "Guest does not match the guest on the stay.");

        public static readonly Error StayNotFound =
            Error.NotFound(
                "Invoice.StayNotFound",
                "The stay associated with the invoice was not found.");

        public static readonly Error GuestNotFound =
            Error.NotFound(
                "Invoice.GuestNotFound",
                "The guest associated with the invoice was not found.");

        public static readonly Error CannotIssue =
            Error.Conflict(
                "Invoice.CannotIssue",
                "The invoice cannot be issued in its current state.");

        public static readonly Error CannotPay =
            Error.Conflict(
                "Invoice.CannotPay",
                "The invoice cannot be paid in its current state.");

        public static readonly Error CannotCancel =
            Error.Conflict(
                "Invoice.CannotCancel",
                "The invoice cannot be cancelled in its current state.");

        public static readonly Error HasNoLineItems =
            Error.Conflict(
                "Invoice.HasNoLineItems",
                "The invoice must contain at least one line item.");

        public static readonly Error AlreadyPaid =
            Error.Conflict(
                "Invoice.AlreadyPaid",
                "The invoice has already been paid.");
    }

    public static class Payment
    {
        public static readonly Error NotFound =
            Error.NotFound(
                "Payment.NotFound",
                "Payment not found.");

        public static readonly Error InvoiceNotFound =
            Error.NotFound(
                "Payment.InvoiceNotFound",
                "The invoice associated with the payment was not found.");

        public static readonly Error AlreadyExists =
            Error.Conflict(
                "Payment.AlreadyExists",
                "A payment already exists for this invoice.");

        public static readonly Error InvalidAmount =
            Error.Validation(
                "Payment.InvalidAmount",
                "The payment amount must be greater than zero.");

        public static readonly Error AmountExceedsInvoice =
            Error.Conflict(
                "Payment.AmountExceedsInvoice",
                "The payment amount cannot exceed the invoice total.");
    }

    public static class Auth
    {

        public static readonly Error NotAuthenticated =
            Error.Unauthorized(
                "Auth.NotAuthenticated",
                "A valid authenticated user is required.");

        public static readonly Error InvalidCredentials =
            Error.Unauthorized(
                "Auth.InvalidCredentials",
                "Invalid email or password.");

        public static readonly Error InvalidRefreshToken =
            Error.Unauthorized(
                "Auth.InvalidRefreshToken",
                "The refresh token is invalid or expired.");

        public static readonly Error UserNotFound =
            Error.NotFound(
                "Auth.UserNotFound",
                "User not found.");

        public static readonly Error EmailAlreadyExists =
            Error.Conflict(
                "Auth.EmailAlreadyExists",
                "A user with this email already exists.");

        public static readonly Error AccountDisabled =
            Error.Forbidden(
                "Auth.AccountDisabled",
                "The user account is disabled.");

        public static readonly Error EmailNotConfirmed =
            Error.Forbidden(
                "Auth.EmailNotConfirmed",
                "The user's email address has not been confirmed.");

        public static readonly Error RefreshTokenRequired =
            Error.Unauthorized(
                "Auth.RefreshTokenRequired",
                "A valid refresh token is required.");
    }

    public static class Authorization
    {
        public static readonly Error Forbidden =
            Error.Forbidden(
                "Authorization.Forbidden",
                "You are not authorized to perform this operation.");

        public static readonly Error RoleRequired =
            Error.Forbidden(
                "Authorization.RoleRequired",
                "You do not have the required role to perform this operation.");
    }
}