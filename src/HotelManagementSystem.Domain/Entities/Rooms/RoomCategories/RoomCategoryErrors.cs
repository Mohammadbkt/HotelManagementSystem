using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;

public static class RoomCategoryErrors
{
    public static readonly Error InvalidId =
        Error.Validation(
            "RoomCategory.InvalidId",
            "Room category ID is invalid.");

    public static readonly Error InvalidHotelId =
        Error.Validation(
            "RoomCategory.InvalidHotelId",
            "Hotel ID is invalid.");

    public static readonly Error NameEmpty =
        Error.Validation(
            "RoomCategory.NameEmpty",
            "Room category name cannot be empty.");

    public static readonly Error NameTooLong =
        Error.Validation(
            "RoomCategory.NameTooLong",
            "Room category name cannot exceed 100 characters.");

    public static readonly Error DescriptionTooLong =
        Error.Validation(
            "RoomCategory.DescriptionTooLong",
            "Room category description cannot exceed 500 characters.");

    public static readonly Error InvalidMaxCapacity =
        Error.Validation(
            "RoomCategory.InvalidMaxCapacity",
            "Maximum capacity must be greater than zero.");

    public static readonly Error PriceRequired = 
        Error.Validation(
            "RoomCategory.ProceRequired", 
            "Room category must have a base price per night"); 

    public static readonly Error PriceMustBePositive = 
        Error.Validation(
            "RoomCategory.PriceMustBePositive",
            "Room category price must be positive"

        );
}