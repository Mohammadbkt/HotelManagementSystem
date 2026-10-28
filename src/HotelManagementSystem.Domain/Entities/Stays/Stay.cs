using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Billing;
using HotelManagementSystem.Domain.Entities.Reservations;

namespace HotelManagementSystem.Domain.Entities.Stays
{
    public sealed class Stay : AuditableEntity
    {
        public Guid GuestId { get; private set; }
        public Guid RoomId { get; private set; }
        public Guid? ReservationId { get; private set; }
        public Guid HotelId { get; private set; }

        public DateTimeOffset CheckInDate { get; private set; }
        public DateTimeOffset? CheckOutDate { get; private set; }

        private Stay()
        {
        }

        private Stay(Guid id, Guid guestId, Guid roomId, Guid hotelId, DateTimeOffset checkInDate, DateTimeOffset? checkOutDate, Guid? reservationId = null) : base(id)
        {
            GuestId = guestId;
            RoomId = roomId;
            HotelId = hotelId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            ReservationId = reservationId;
        }

        public static Result<Stay> Create(Guid guestId, Guid roomId, Guid hotelId, DateTimeOffset checkInDate, Guid? reservationId = null)
        {
            if (guestId == Guid.Empty)
                return Result<Stay>.Failure(StayErrors.InvalidGuestId);

            if (roomId == Guid.Empty)
                return Result<Stay>.Failure(StayErrors.InvalidRoomId);

            if (hotelId == Guid.Empty)
                return Result<Stay>.Failure(StayErrors.InvalidHotelId);

            if (checkInDate == default)
                return Result<Stay>.Failure(StayErrors.InvalidCheckInDate);


            return Result<Stay>.Success(new Stay(Guid.NewGuid(), guestId, roomId, hotelId, checkInDate, null, reservationId));
        }

        public Result CheckOut(DateTimeOffset checkOutDate)
        {
            if (CheckOutDate.HasValue)
                return Result.Failure(StayErrors.AlreadyCheckedOut);

            if (checkOutDate == default)
                return Result.Failure(StayErrors.InvalidCheckOutTime);

            if (checkOutDate < CheckInDate)
                return Result.Failure(StayErrors.InvalidCheckOutTime);

            CheckOutDate = checkOutDate;

            AddDomainEvent(new StayCheckedOutEvent(Id, GuestId, RoomId, checkOutDate));


            return Result.Success();
        }

        public static Result<Stay> CreateFromReservation(Reservation reservation, Guid RoomId, DateTimeOffset checkInDate)
        {
            if (reservation.Status != ReservationStatus.Confirmed)
                return Result<Stay>.Failure(StayErrors.InvalidReservationStatus);

            if (reservation.GuestId == Guid.Empty)
                return Result<Stay>.Failure(StayErrors.InvalidGuestId);

            if (reservation.HotelId == Guid.Empty)
                return Result<Stay>.Failure(StayErrors.InvalidHotelId);

            if (checkInDate == default)
                return Result<Stay>.Failure(StayErrors.InvalidCheckInDate);

            if (reservation.Id == Guid.Empty)
                return Result<Stay>.Failure(StayErrors.InvalidReservationId);

            return Result<Stay>.Success(new Stay(Guid.NewGuid(), reservation.GuestId, RoomId, reservation.HotelId, checkInDate, null, reservation.Id));
        }
    }
}