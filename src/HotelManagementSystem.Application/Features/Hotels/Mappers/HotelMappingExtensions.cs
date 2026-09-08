using System;
using System.Linq;
using System.Linq.Expressions;
using HotelManagementSystem.Application.Features.Hotels.Dtos;
using HotelManagementSystem.Domain.Entities.Hotels;

namespace HotelManagementSystem.Application.Features.Hotels.Mappers
{
    public static class HotelMappingExtensions
    {
        public static readonly Expression<Func<Hotel, HotelDto>> ProjectionExpression =
            h => new HotelDto(
                h.Id,
                h.Name,
                h.Address.AddressLine1,
                h.Address.AddressLine2,
                h.Address.City,
                h.Address.StateOrProvince,
                h.Address.PostalCode,
                h.Address.Country,
                h.PhoneNumber.Value);

        private static readonly Func<Hotel, HotelDto> _compiledProjection = ProjectionExpression.Compile();

        public static HotelDto ToDto(this Hotel hotel) => _compiledProjection(hotel);

        public static Expression<Func<Hotel, HotelDto>> ToDtoExpression() => ProjectionExpression;

        public static IQueryable<HotelDto> ToHotelDto(this IQueryable<Hotel> query)
            => query.Select(ProjectionExpression);
    }
}