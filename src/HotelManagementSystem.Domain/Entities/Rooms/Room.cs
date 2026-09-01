using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;

namespace HotelManagementSystem.Domain.Entities.Rooms
{
    public sealed class Room : AuditableEntity
    {
        public Guid HotelId { get; private set; }
        public string RoomNumber { get; private set; } = string.Empty;
        public RoomStatus Status { get; private set; }
        public Guid CategoryId { get; private set; }
        public string? StatusReason { get; private set; }

        private Room() { }

        private Room(Guid id, Guid hotelId, string roomNumber, Guid categoryId) : base(id)
        {
            HotelId = hotelId;
            RoomNumber = roomNumber;
            CategoryId = categoryId;
            Status = RoomStatus.CleanVacant;
        }

        public static Result<Room> Create(Guid hotelId, string roomNumber, Guid categoryId)
        {
            if (hotelId == Guid.Empty)
                return RoomErrors.InvalidHotelId;
            if (string.IsNullOrWhiteSpace(roomNumber))
                return RoomErrors.RoomNumberRequired;

            if (roomNumber.Trim().Length > 20)
                return RoomErrors.RoomNumberTooLong;

            if (categoryId == Guid.Empty)
                return RoomErrors.InvalidCategoryId;

            return new Room(Guid.NewGuid(), hotelId, roomNumber.Trim(), categoryId);

        }

        public Result CheckIn()
        {
            if (Status != RoomStatus.CleanVacant)
                return RoomErrors.RoomNotAvailable;

            Status = RoomStatus.Occupied;
            return Result.Success();
        }

        public Result CheckOut()
        {
            if (Status != RoomStatus.Occupied)
                return RoomErrors.RoomNotOccupied;

            Status = RoomStatus.DirtyVacant;
            return Result.Success();
        }

        public Result MarkAsClean()
        {
            if (Status != RoomStatus.DirtyVacant)
                return RoomErrors.RoomNotDirty;

            Status = RoomStatus.CleanVacant;
            return Result.Success();
        }

        public Result TakeOutOfOrder(string? reason)
        {
            if (Status == RoomStatus.Occupied)
                return RoomErrors.CannotTakeOccupiedRoomOutOfOrder;

            if (reason?.Trim().Length > 500)
                return RoomErrors.StatusReasonTooLong;

            StatusReason = reason?.Trim();
            Status = RoomStatus.OutOfOrder;
            return Result.Success();
        }

        public Result ReturnToService()
        {
            if (Status != RoomStatus.OutOfOrder && Status != RoomStatus.Maintenance)
                return RoomErrors.RoomNotOutOfOrder;

            Status = RoomStatus.CleanVacant;
            StatusReason = null;
            return Result.Success();
        }

        public Result MoveToMaintenance(string? reason)
        {
            if (Status == RoomStatus.Occupied)
                return RoomErrors.CannotMoveOccupiedToMaintenance;

            if (reason?.Trim().Length > 500)
                return RoomErrors.StatusReasonTooLong;


            StatusReason = reason?.Trim();
            Status = RoomStatus.Maintenance;
            return Result.Success();
        }

    }
}