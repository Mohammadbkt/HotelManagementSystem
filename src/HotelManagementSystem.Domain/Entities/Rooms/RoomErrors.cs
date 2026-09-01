using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Rooms
{
    public class RoomErrors
    {

        public static Error InvalidHotelId => 
            Error.Validation(
                "Room.InvalidHotelId", 
                "Hotel ID is required.");
        public static Error RoomNumberRequired 
            => Error.Validation(
                "Room.RoomNumberRequired", 
                "Room number is required.");
        public static Error RoomNumberTooLong => 
            Error.Validation(
                "Room.RoomNumberTooLong", 
                "Room number cannot exceed 20 characters.");
        public static Error InvalidCategoryId => 
            Error.Validation(
                "Room.InvalidCategoryId", 
                "Room category is required.");
        public static Error RoomNotAvailable => 
            Error.Validation(
                "Room.NotAvailable", 
                "Room is not available for assignment.");
        public static Error RoomNotOccupied => 
            Error.Validation(
                "Room.NotOccupied", 
                "Room is not currently occupied.");
        public static Error RoomNotDirty => 
            Error.Validation(
                "Room.NotDirty", 
                "Room is not in a dirty state.");
        public static Error CannotTakeOccupiedRoomOutOfOrder => 
            Error.Validation(
                "Room.CannotTakeOccupied", 
                "Cannot take an occupied room out of order.");
        public static Error CannotMoveOccupiedToMaintenance => 
            Error.Validation(
                "Room.CannotMoveToMaintenance", 
                "Cannot move an occupied room to maintenance.");
        public static Error RoomNotOutOfOrder => 
            Error.Validation(
                "Room.NotOutOfOrder", 
                "Room is not out of order.");
        public static Error StatusReasonTooLong =>
            Error.Validation(
                "Room.StatusReasonTooLong",
                "Status reason is too long"); 

    }

}