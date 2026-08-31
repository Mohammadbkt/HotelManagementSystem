using System;
using System.Linq;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.ValueObjects.Address
{
    public readonly record struct Address
    {
        public string AddressLine1 { get; }
        public string? AddressLine2 { get; }
        public string City { get; }
        public string? StateOrProvince { get; }
        public string? PostalCode { get; }
        public string Country { get; }

        private Address(
            string addressLine1,
            string? addressLine2,
            string city,
            string? stateOrProvince,
            string? postalCode,
            string country)
        {
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            City = city;
            StateOrProvince = stateOrProvince;
            PostalCode = postalCode;
            Country = country;
        }

        public static Result<Address> Create(
            string addressLine1,
            string city,
            string country,
            string? addressLine2 = null,
            string? stateOrProvince = null,
            string? postalCode = null)
        {
            if (string.IsNullOrWhiteSpace(addressLine1))
                return AddressErrors.AddressLine1Empty;

            if (string.IsNullOrWhiteSpace(city))
                return AddressErrors.CityEmpty;

            if (string.IsNullOrWhiteSpace(country))
                return AddressErrors.CountryEmpty;

            if (addressLine1.Trim().Length > 200)
                return AddressErrors.AddressLine1TooLong;

            if (addressLine2?.Trim().Length > 200)
                return AddressErrors.AddressLine2TooLong;

            if (city.Trim().Length > 100)
                return AddressErrors.CityTooLong;

            if (stateOrProvince?.Trim().Length > 100)
                return AddressErrors.StateOrProvinceTooLong;

            if (postalCode?.Trim().Length > 20)
                return AddressErrors.PostalCodeTooLong;

            if (country.Trim().Length > 100)
                return AddressErrors.CountryTooLong;

            return new Address(
                addressLine1.Trim(),
                string.IsNullOrWhiteSpace(addressLine2) ? null : addressLine2.Trim(),
                city.Trim(),
                string.IsNullOrWhiteSpace(stateOrProvince) ? null : stateOrProvince.Trim(),
                string.IsNullOrWhiteSpace(postalCode) ? null : postalCode.Trim(),
                country.Trim());
        }

        public override string ToString()
        {
            var parts = new[] { AddressLine1, AddressLine2, City, StateOrProvince, PostalCode, Country };
            return string.Join(", ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));
        }
    }
}