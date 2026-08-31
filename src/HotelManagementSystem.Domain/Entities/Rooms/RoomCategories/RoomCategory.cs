using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;

namespace HotelManagementSystem.Domain.Entities.Rooms.RoomCategories
{
    public sealed class RoomCategory : AuditableEntity
    {
        public Guid HotelId { get; private set; }
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; } = default;
        public int MaxCapacity { get; private set; }
        public Money BasePricePerNight { get; private set; } = default!;

        private RoomCategory()
        {

        }

        private RoomCategory(Guid Id, Guid hotelId, string name, string? description, int maxCapacity, Money basePricePerNight) : base(Id)
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
                return RoomCategoryErrors.InvalidHotelId;

            if (string.IsNullOrWhiteSpace(name))
                return RoomCategoryErrors.NameEmpty;

            if (name.Trim().Length > 100)
                return RoomCategoryErrors.NameTooLong;

            if (description?.Length > 500)
                return RoomCategoryErrors.DescriptionTooLong;

            if (maxCapacity <= 0)
                return RoomCategoryErrors.InvalidMaxCapacity;

            if (basePricePerNight.Amount <= 0)
                return RoomCategoryErrors.PriceMustBePositive;

            return new RoomCategory(
                Guid.NewGuid(),
                hotelId,
                name.Trim(),
                description?.Trim(),
                maxCapacity,
                basePricePerNight);
        }

        public Result UpdateDetails(string name, string? description, int maxCapacity)
        {
            if (string.IsNullOrWhiteSpace(name))
                return RoomCategoryErrors.NameEmpty;

            if (name.Trim().Length > 100)
                return RoomCategoryErrors.NameTooLong;

            if (description?.Length > 500)
                return RoomCategoryErrors.DescriptionTooLong;

            if (maxCapacity <= 0)
                return RoomCategoryErrors.InvalidMaxCapacity;

            Name = name.Trim();
            Description = description?.Trim();
            MaxCapacity = maxCapacity;

            return Result.Success();
        }

        public Result UpdateBasePrice(Money newPrice)
        {
            if (newPrice.Amount <= 0)
                return RoomCategoryErrors.PriceMustBePositive;

            BasePricePerNight = newPrice;
            return Result.Success();
        }
    }
}
