using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;

namespace HotelManagementSystem.Domain.Entities.Rooms.RoomCategories
{
    public sealed class RoomCategory : SoftDeletableEntity
    {
        public Guid HotelId { get; private set; }
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; } = default;
        public int MaxCapacity { get; private set; }
        public Money BasePricePerNight { get; private set; } = default!;

        private RoomCategory()
        {

        }

        private RoomCategory(Guid id, Guid hotelId, string name, string? description, int maxCapacity, Money basePricePerNight) : base(id)
        {
            HotelId = hotelId;
            Name = name;
            Description = description;
            MaxCapacity = maxCapacity;
            BasePricePerNight = basePricePerNight;
        }

        public static Result<RoomCategory> Create(Guid hotelId, string name, string? description, int maxCapacity, Money basePricePerNight)
        {
            if (hotelId == Guid.Empty)
                return Result<RoomCategory>.Failure(RoomCategoryErrors.InvalidHotelId);

            if (string.IsNullOrWhiteSpace(name))
                return Result<RoomCategory>.Failure(RoomCategoryErrors.NameEmpty);

            if (name.Trim().Length > 100)
                return Result<RoomCategory>.Failure(RoomCategoryErrors.NameTooLong);

            if (description?.Trim().Length > 500)
                return Result<RoomCategory>.Failure(RoomCategoryErrors.DescriptionTooLong);

            if (maxCapacity <= 0)
                return Result<RoomCategory>.Failure(RoomCategoryErrors.InvalidMaxCapacity);

            if (basePricePerNight.Amount <= 0)
                return Result<RoomCategory>.Failure(RoomCategoryErrors.PriceMustBePositive);

            return Result<RoomCategory>.Success(new RoomCategory(
                Guid.NewGuid(),
                hotelId,
                name.Trim(),
                description?.Trim(),
                maxCapacity,
                basePricePerNight));
        }

        public Result UpdateDetails(string name, string? description, int maxCapacity)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure(RoomCategoryErrors.NameEmpty);

            if (name.Trim().Length > 100)
                return Result.Failure(RoomCategoryErrors.NameTooLong);

            if (description?.Trim().Length > 500)
                return Result.Failure(RoomCategoryErrors.DescriptionTooLong);

            if (maxCapacity <= 0)
                return Result.Failure(RoomCategoryErrors.InvalidMaxCapacity);

            Name = name.Trim();
            Description = description?.Trim();
            MaxCapacity = maxCapacity;

            return Result.Success();
        }

        public Result UpdateBasePrice(Money newPrice)
        {
            if (newPrice.Amount <= 0)
                return Result.Failure(RoomCategoryErrors.PriceMustBePositive);

            BasePricePerNight = newPrice;
            return Result.Success();
        }
    }
}
