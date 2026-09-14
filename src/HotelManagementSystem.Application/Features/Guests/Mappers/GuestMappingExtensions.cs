using System.Linq.Expressions;
using HotelManagementSystem.Application.Features.Guests.Dtos;
using HotelManagementSystem.Domain.Entities.Guests;

namespace HotelManagementSystem.Application.Features.Guests.Mappers
{
    public static class GuestMappingExtensions
    {
        public static readonly Expression<Func<Guest, GuestDto>> ProjectionExpression =
            g => new GuestDto(
                g.Id,
                g.FirstName,
                g.LastName,
                g.Email != null ? g.Email.Value.Value : null,
                g.PhoneNumber != null ? g.PhoneNumber.Value.Value : null,
                g.DateOfBirth
            );

            private static readonly Func<Guest, GuestDto> _compiledProjection = ProjectionExpression.Compile();

            public static GuestDto ToDto(this Guest guest) =>_compiledProjection(guest);

            public static IQueryable<GuestDto> ToGuestDto(this IQueryable<Guest> query) 
                => query.Select(ProjectionExpression); 
    }
}