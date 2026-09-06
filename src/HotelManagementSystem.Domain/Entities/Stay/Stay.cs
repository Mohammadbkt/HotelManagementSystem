using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Reservations;

namespace HotelManagementSystem.Domain.Entities.Stay
{
    public sealed class Stay : AuditableEntity
    {
        public Guid GuestId { get; private set; }
        public Guid RoomId { get; private set; }
        public Guid? ReservationId { get; private set; }
        public DateTimeOffset CheckInDate { get; private set; }
        public DateTimeOffset? CheckOutDate { get; private set; }

        private Stay()
        {
        }

        private Stay(Guid id, Guid guestId, Guid roomId, DateTimeOffset checkInDate, DateTimeOffset? checkOutDate, Guid? reservationId = null) : base(id)
        {
            GuestId = guestId;
            RoomId = roomId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            ReservationId = reservationId;
        }

        public static Result<Stay> Create(Guid guestId, Guid roomId, DateTimeOffset checkInDate, Guid? reservationId = null)
        {
            if (guestId == Guid.Empty)
                return StayErrors.InvalidGuestId;
            if (roomId == Guid.Empty)
                return StayErrors.InvalidRoomId;

            if (checkInDate == default)
                return StayErrors.InvalidCheckInDate;


            return new Stay(Guid.NewGuid(), guestId, roomId, checkInDate, null, reservationId);
        }

        public Result CheckOut(DateTimeOffset checkOutDate)
        {
            if (CheckOutDate.HasValue)
                return StayErrors.AlreadyCheckedOut;

            if (checkOutDate == default)
                return StayErrors.InvalidCheckOutTime;

            if (checkOutDate < CheckInDate)
                return StayErrors.InvalidCheckOutTime;

            CheckOutDate = checkOutDate;

            AddDomainEvent(new StayCheckedOutEvent(Id, GuestId, RoomId, checkOutDate));


            return Result.Success();
        }

        public static Result<Stay> CreateFromReservation(Reservation reservation, DateTimeOffset checkInDate)
        {
            if (reservation.GuestId == Guid.Empty)
                return StayErrors.InvalidGuestId;
            if (reservation.RoomId == Guid.Empty)
                return StayErrors.InvalidRoomId;

            if (checkInDate == default)
                return StayErrors.InvalidCheckInDate;

            if (reservation.Id == Guid.Empty)
                return StayErrors.InvalidReservationId;

            return new Stay(Guid.NewGuid(), reservation.GuestId, reservation.RoomId, checkInDate, null, reservation.Id);
        }
    }
}