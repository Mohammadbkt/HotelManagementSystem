using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Address;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;

namespace HotelManagementSystem.Domain.Entities.Hotels
{
    public sealed class Hotel : AuditableEntity
    {
        public string Name { get; private set; } = string.Empty;
        public Address Address { get; private set; } = default!;
        public PhoneNumber PhoneNumber { get; private set; } = default!;

        private Hotel()
        {

        }

        private Hotel(Guid id, string name, Address address, PhoneNumber phone) : base(id)
        {
            Name = name;
            Address = address;
            PhoneNumber = phone;
        }

        public static Result<Hotel> Create(string name, Address address, PhoneNumber phone)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Hotel>.Failure(Error.Validation("Hotel.NameRequired", "Hotel name is required."));

            return Result<Hotel>.Success(new Hotel(Guid.NewGuid(), name.Trim(), address, phone));
        }

        
    public Result UpdateInfo(
        string name,
        Address address,
        PhoneNumber phone)
        {
            if(string.IsNullOrWhiteSpace(name))
                return Result.Failure(Error.Validation("Hotel.NameRequired", "Hotel name is required."));
            
            Name = name.Trim();
            Address = address;
            PhoneNumber = phone;

            return Result.Success();
        }

    }
}