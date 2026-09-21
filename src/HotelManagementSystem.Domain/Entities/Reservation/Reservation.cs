using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Reservations
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

        private Reservation(Guid id, Guid guestId, Guid roomId, DateOnly checkInDate, DateOnly checkOutDate) : base(id)
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
                return Result<Reservation>.Failure(ReservationErrors.InvalidGuestId);
            if (roomId == Guid.Empty)
                return Result<Reservation>.Failure(ReservationErrors.InvalidRoomId);
            if (checkInDate >= checkOutDate)
                return Result<Reservation>.Failure(ReservationErrors.InvalidCheckInOutDates);

            return Result<Reservation>.Success(new Reservation(Guid.NewGuid(), guestId, roomId, checkInDate, checkOutDate));
        }

        public Result Update(Guid roomId, DateOnly checkInDate, DateOnly checkOutDate)
        {
            if (Status != ReservationStatus.Pending)
                return Result.Failure(ReservationErrors.CannotUpdateReservation);

            if (roomId == Guid.Empty)
                return Result.Failure(ReservationErrors.InvalidRoomId);

            if (checkInDate >= checkOutDate)
                return Result.Failure(ReservationErrors.InvalidCheckInOutDates);

            RoomId = roomId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;

            return Result.Success();
        }

        public Result Confirm()
        {
            if (Status != ReservationStatus.Pending)
                return Result.Failure(ReservationErrors.CannotConfirmReservation);
            Status = ReservationStatus.Confirmed;

            return Result.Success();
        }

        public Result Cancel()
        {
            if (Status != ReservationStatus.Pending && Status != ReservationStatus.Confirmed)
                return Result.Failure(ReservationErrors.CannotCancelReservation);

            Status = ReservationStatus.Cancelled;

            return Result.Success();
        }

        public Result MarkAsCheckedIn(DateOnly today)
        {
            if (Status != ReservationStatus.Confirmed)
                return Result.Failure(ReservationErrors.CannotCheckIn);

            if (today < CheckInDate)
                return Result.Failure(ReservationErrors.CannotCheckIn);
                
            if (today >= CheckOutDate)
                return Result.Failure(ReservationErrors.CannotCheckIn);


            Status = ReservationStatus.CheckedIn;
            return Result.Success();
        }

        public Result MarkAsComplete()
        {
            if (Status != ReservationStatus.CheckedIn)
                return Result.Failure(ReservationErrors.CannotCompleteBeforeCheckIn);

            Status = ReservationStatus.Completed;
            return Result.Success();

        }

        public Result MarkAsNoShow()
        {
            if (Status != ReservationStatus.Confirmed)
                return Result.Failure(ReservationErrors.CannotMarkNoShow);

            Status = ReservationStatus.NoShow;
            return Result.Success();
        }
    }
}