using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Rooms
{
    public sealed class Room : SoftDeletableEntity
    {
        public Guid HotelId { get; private set; }
        public string RoomNumber { get; private set; } = string.Empty;
        public RoomStatus Status { get; private set; }
        public Guid RoomCategoryId { get; private set; }
        public string? StatusReason { get; private set; }

        private Room() { }

        private Room(Guid id, Guid hotelId, string roomNumber, Guid categoryId) : base(id)
        {
            HotelId = hotelId;
            RoomNumber = roomNumber;
            RoomCategoryId = categoryId;
            Status = RoomStatus.CleanVacant;
        }

        public static Result<Room> Create(Guid hotelId, string roomNumber, Guid categoryId)
        {
            if (hotelId == Guid.Empty)
                return Result<Room>.Failure(RoomErrors.InvalidHotelId);
            if (string.IsNullOrWhiteSpace(roomNumber))
                return Result<Room>.Failure(RoomErrors.RoomNumberRequired);

            if (roomNumber.Trim().Length > 20)
                return Result<Room>.Failure(RoomErrors.RoomNumberTooLong);

            if (categoryId == Guid.Empty)
                return Result<Room>.Failure(RoomErrors.InvalidCategoryId);

            return Result<Room>.Success(new Room(Guid.NewGuid(), hotelId, roomNumber.Trim(), categoryId));
        }

        public Result ChangeRoomNumber(string roomNumber)
        {
            if (Status != RoomStatus.CleanVacant)
                return Result.Failure(RoomErrors.CannotUpdateOccupiedRoom);


            if (string.IsNullOrWhiteSpace(roomNumber))
                return Result.Failure(RoomErrors.RoomNumberRequired);

            if (roomNumber.Trim().Length > 20)
                return Result.Failure(RoomErrors.RoomNumberTooLong);

            RoomNumber = roomNumber.Trim();

            return Result.Success();
        }

        public Result ChangeCategory(Guid roomCategoryId)
        {
            if (Status == RoomStatus.Occupied)
                return Result.Failure(RoomErrors.CannotUpdateOccupiedRoom);


            if (roomCategoryId == Guid.Empty)
                return Result.Failure(RoomErrors.InvalidCategoryId);

            RoomCategoryId = roomCategoryId;

            return Result.Success();
        }

        public Result CheckIn()
        {
            if (Status != RoomStatus.CleanVacant)
                return Result.Failure(RoomErrors.RoomNotAvailable);

            Status = RoomStatus.Occupied;
            return Result.Success();
        }

        public Result CheckOut()
        {
            if (Status != RoomStatus.Occupied)
                return Result.Failure(RoomErrors.RoomNotOccupied);

            Status = RoomStatus.DirtyVacant;
            return Result.Success();
        }

        public Result MarkAsClean()
        {
            if (Status != RoomStatus.DirtyVacant)
                return Result.Failure(RoomErrors.RoomNotDirty);

            Status = RoomStatus.CleanVacant;
            return Result.Success();
        }

        public Result TakeOutOfOrder(string? reason)
        {
            if (Status == RoomStatus.Occupied)
                return Result.Failure(RoomErrors.CannotTakeOccupiedRoomOutOfOrder);

            var reasonResult = ValidateStatusReason(reason);
            if (reasonResult.IsError)
                return Result.Failure(reasonResult.Errors);

            StatusReason = reasonResult.Value;
            Status = RoomStatus.OutOfOrder;
            return Result.Success();
        }

        public Result ReturnToService()
        {
            if (Status != RoomStatus.OutOfOrder && Status != RoomStatus.Maintenance)
                return Result.Failure(RoomErrors.RoomNotOutOfOrder);

            Status = RoomStatus.CleanVacant;
            StatusReason = null;
            return Result.Success();
        }

        public Result MoveToMaintenance(string? reason)
        {
            if (Status == RoomStatus.Occupied)
                return Result.Failure(RoomErrors.CannotMoveOccupiedToMaintenance);

            var reasonResult = ValidateStatusReason(reason);
            if (reasonResult.IsError)
                return Result.Failure(reasonResult.Errors);

            StatusReason = reasonResult.Value;
            Status = RoomStatus.Maintenance;
            return Result.Success();
        }

        private static Result<string?> ValidateStatusReason(string? reason)
        {
            if (reason?.Trim().Length > 500)
                return Result<string?>.Failure(RoomErrors.StatusReasonTooLong);

            return Result<string?>.Success(reason?.Trim());
        }

    }
}