using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Stay
{
    public static class StayErrors
    {
        public static Error InvalidGuestId =>
            Error.Validation(
                "Stay.InvalidGuestId", 
                "Invalid guest ID.");
            
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
    }
}
           