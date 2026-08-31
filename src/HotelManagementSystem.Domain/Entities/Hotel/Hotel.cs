using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Address;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;

namespace HotelManagementSystem.Domain.Entities.Hotel
{
    public sealed class Hotel : AuditableEntity
    {
        public string Name { get; private set; } = string.Empty;
        public Address Address { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }

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
                return Error.Validation("Hotel.NameRequired", "Hotel name is required.");

            return new Hotel(Guid.NewGuid(), name.Trim(), address, phone);
        }

    }
}