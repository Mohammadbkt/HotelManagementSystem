using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;

namespace HotelManagementSystem.Domain.Entities.Reservations
{
    public sealed class Reservation : AuditableEntity
    {
        public Guid GuestId { get; private set; }
        public Guid RoomCategoryId { get; private set; }
        public Guid HotelId { get; private set; }
        public Guid? RoomId { get; private set; }

        public string ReservationNumber { get; private set; } = default!;

        public ReservationStatus Status { get; private set; }
        public Money BasePricePerNight { get; private set; } = default!;
        public Money TotalAmount { get; private set; } = default!;

        public DateOnly CheckInDate { get; private set; }
        public DateOnly CheckOutDate { get; private set; }

        public int NumberOfAdults { get; private set; }
        public int NumberOfChildren { get; private set; }

        public DateTimeOffset? CancelledAt { get; private set; }
        public string? CancellationReason { get; private set; }
        public string? SpecialRequests { get; private set; }

        private Reservation()
        {
        }

        private Reservation(
                Guid id,
                Guid guestId,
                Guid hotelId,
                Guid roomCategoryId,
                Guid? roomId,
                string reservationNumber,
                DateOnly checkInDate,
                DateOnly checkOutDate,
                int numberOfAdults,
                int numberOfChildren,
                string? specialRequests,
                Money basePricePerNight,
                Money totalAmount) : base(id)
        {
            GuestId = guestId;
            HotelId = hotelId;
            RoomCategoryId = roomCategoryId;
            RoomId = roomId;
            ReservationNumber = reservationNumber;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            NumberOfAdults = numberOfAdults;
            NumberOfChildren = numberOfChildren;
            SpecialRequests = specialRequests;
            BasePricePerNight = basePricePerNight;
            TotalAmount = totalAmount;
            Status = ReservationStatus.Pending;
        }

        public static Result<Reservation> Create(
        Guid guestId,
        Guid hotelId,
        Guid roomCategoryId,
        Guid? roomId,
        string reservationNumber,
        DateOnly checkInDate,
        DateOnly checkOutDate,
        int numberOfAdults,
        int numberOfChildren,
        Money basePricePerNight,
        Money totalAmount,
        string? specialRequests = null)
        {
            if (guestId == Guid.Empty)
                return Result<Reservation>.Failure(
                    ReservationErrors.InvalidGuestId);

            if (hotelId == Guid.Empty)
                return Result<Reservation>.Failure(
                    ReservationErrors.InvalidHotelId);

            if (roomCategoryId == Guid.Empty)
                return Result<Reservation>.Failure(
                    ReservationErrors.InvalidRoomCategoryId);

            if (roomId.HasValue && roomId.Value == Guid.Empty)
                return Result<Reservation>.Failure(
                    ReservationErrors.InvalidRoomId);

            if (string.IsNullOrWhiteSpace(reservationNumber))
                return Result<Reservation>.Failure(
                    ReservationErrors.InvalidReservationNumber);

            if (checkInDate >= checkOutDate)
                return Result<Reservation>.Failure(
                    ReservationErrors.InvalidCheckInOutDates);

            if (numberOfAdults < 1 || numberOfChildren < 0)
                return Result<Reservation>.Failure(
                    ReservationErrors.InvalidGuestCount);

            if (specialRequests?.Length > 1000)
                return Result<Reservation>.Failure(
                    ReservationErrors.SpecialRequestsTooLong);

            if (basePricePerNight.Amount <= 0)
                return Result<Reservation>.Failure(ReservationErrors.InvalidPricing);

            if (totalAmount.Amount <= 0)
                return Result<Reservation>.Failure(ReservationErrors.InvalidPricing);

            return Result<Reservation>.Success(
                new Reservation(
                    Guid.NewGuid(),
                    guestId,
                    hotelId,
                    roomCategoryId,
                    roomId,
                    reservationNumber.Trim(),
                    checkInDate,
                    checkOutDate,
                    numberOfAdults,
                    numberOfChildren,
                    string.IsNullOrWhiteSpace(specialRequests)
                        ? null
                        : specialRequests.Trim(),
                    basePricePerNight,
                    totalAmount));
        }

        public Result Update(
       Guid roomCategoryId,
       DateOnly checkInDate,
       DateOnly checkOutDate,
       int numberOfAdults,
       int numberOfChildren,
       string? specialRequests,
       Money basePricePerNight,
       Money totalAmount)
        {
            if (Status != ReservationStatus.Pending)
                return Result.Failure(
                    ReservationErrors.CannotUpdateReservation);

            if (roomCategoryId == Guid.Empty)
                return Result.Failure(
                    ReservationErrors.InvalidRoomCategoryId);

            if (checkInDate >= checkOutDate)
                return Result.Failure(
                    ReservationErrors.InvalidCheckInOutDates);

            if (numberOfAdults < 1 || numberOfChildren < 0)
                return Result.Failure(
                    ReservationErrors.InvalidGuestCount);

            if (specialRequests?.Length > 1000)
                return Result.Failure(
                    ReservationErrors.SpecialRequestsTooLong);

            if (basePricePerNight.Amount <= 0)
                return Result.Failure(ReservationErrors.InvalidPricing);

            if (totalAmount.Amount <= 0)
                return Result.Failure(ReservationErrors.InvalidPricing);


            if (RoomCategoryId != roomCategoryId)
                RoomId = null;

            RoomCategoryId = roomCategoryId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            NumberOfAdults = numberOfAdults;
            NumberOfChildren = numberOfChildren;
            SpecialRequests = string.IsNullOrWhiteSpace(specialRequests)
                ? null
                : specialRequests.Trim();
            BasePricePerNight = basePricePerNight;
            TotalAmount = totalAmount;

            return Result.Success();
        }

        public Result AssignRoom(Guid roomId)
        {
            if (roomId == Guid.Empty)
                return Result.Failure(
                    ReservationErrors.InvalidRoomId);

            if (Status != ReservationStatus.Pending &&
                Status != ReservationStatus.Confirmed)
            {
                return Result.Failure(
                    ReservationErrors.CannotAssignRoom);
            }

            RoomId = roomId;

            return Result.Success();
        }


        public Result Confirm()
        {
            if (Status != ReservationStatus.Pending)
                return Result.Failure(ReservationErrors.CannotConfirmReservation);
            Status = ReservationStatus.Confirmed;

            return Result.Success();
        }

        public Result Cancel(DateTimeOffset cancelledAt, string? reason)
        {
            if (Status != ReservationStatus.Pending && Status != ReservationStatus.Confirmed)
                return Result.Failure(ReservationErrors.CannotCancelReservation);

            if (reason?.Length > 500)
                return Result.Failure(
                    ReservationErrors.CancellationReasonTooLong);

            Status = ReservationStatus.Cancelled;
            CancelledAt = cancelledAt;
            CancellationReason = string.IsNullOrWhiteSpace(reason)
                ? null
                : reason.Trim();


            return Result.Success();
        }

        public Result MarkAsCheckedIn(DateOnly today)
        {
            if (Status != ReservationStatus.Confirmed ||
            today < CheckInDate ||
            today >= CheckOutDate)
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