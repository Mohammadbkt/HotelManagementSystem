using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Reservation
{
    public sealed class Reservation : AuditableEntity
    {
        public Guid GuestId { get; private set; }
        public Guid RoomId { get; private set; }
        public ReservationStatus Status { get; private set; }
        public DateOnly CheckInDate { get; private set; }
        public DateOnly CheckOutDate { get; private set; }

        private Reservation()
        {
        }

        private Reservation(Guid guestId, Guid roomId, DateOnly checkInDate, DateOnly checkOutDate)
        {
            GuestId = guestId;
            RoomId = roomId;
            Status = ReservationStatus.Pending;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
        }

        public static Result<Reservation> Create(Guid guestId, Guid roomId, DateOnly checkInDate, DateOnly checkOutDate)
        {
            if (guestId == Guid.Empty)
                return ReservationErrors.InvalidGuestId;
            if (roomId == Guid.Empty)
                return ReservationErrors.InvalidRoomId;
            if (checkInDate >= checkOutDate)
                return ReservationErrors.InvalidCheckInOutDates;

            return new Reservation(guestId, roomId, checkInDate, checkOutDate);
        }

        public Result Update(Guid roomId, DateOnly checkInDate, DateOnly checkOutDate)
        {
            if (Status != ReservationStatus.Pending)
                return ReservationErrors.CannotUpdateReservation;

            if (roomId == Guid.Empty)
                return ReservationErrors.InvalidRoomId;

            if (checkInDate >= checkOutDate)
                return ReservationErrors.InvalidCheckInOutDates;

            RoomId = roomId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;

            return Result.Success();
        }

        public Result Confirm()
        {
            if (Status != ReservationStatus.Pending)
                return ReservationErrors.CannotConfirmReservation;
            Status = ReservationStatus.Confirmed;

            return Result.Success();
        }

        public Result Cancel()
        {
            if (Status == ReservationStatus.Cancelled)
                return ReservationErrors.ReservationAlreadyCancelled;

            Status = ReservationStatus.Cancelled;

            return Result.Success();
        }
    }
}