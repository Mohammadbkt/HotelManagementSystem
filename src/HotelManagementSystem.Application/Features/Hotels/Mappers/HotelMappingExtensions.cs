using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HotelManagementSystem.Application.Features.Hotels.Dtos;
using HotelManagementSystem.Domain.Entities.Hotels;

namespace HotelManagementSystem.Application.Features.Hotels.Mappers
{
    public static class HotelMappingExtensions
    {
        public static HotelDto ToDto(this Hotel hotel)
            => new(
                hotel.Id,
                hotel.Name,
                hotel.Address.AddressLine1,
                hotel.Address.AddressLine2,
                hotel.Address.City,
                hotel.Address.StateOrProvince,
                hotel.Address.PostalCode,
                hotel.Address.Country,
                hotel.PhoneNumber.Value);

        public static Expression<Func<Hotel, HotelDto>> ToDtoExpression()
        {
            return  h => new HotelDto(h.Id,
            h.Name,
            h.Address.AddressLine1,
            h.Address.AddressLine2,
            h.Address.City,
            h.Address.StateOrProvince,
            h.Address.PostalCode,
            h.Address.Country,
            h.PhoneNumber.Value);
        }

    }
}