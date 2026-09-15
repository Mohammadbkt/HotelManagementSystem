using System.Linq.Expressions;
using HotelManagementSystem.Application.Features.Stays.Dtos;
using HotelManagementSystem.Domain.Entities.Stays;

namespace HotelManagementSystem.Application.Features.Stays.Mappers
{
    public static class StayMappersExtension
    {
        public static readonly Expression<Func<Stay, StayDto>> ProjectionExpression =
        s => new StayDto(
            s.Id,
            s.GuestId,
            s.RoomId,
            s.ReservationId,
            s.CheckInDate,
            s.CheckOutDate
        );

        private static readonly Func<Stay, StayDto> _compiledProjection = ProjectionExpression.Compile();

        public static StayDto ToDto(this Stay stay)
            => _compiledProjection(stay);

        public static IQueryable<StayDto> ToStayDto(this IQueryable<Stay> query)
            => query.Select(ProjectionExpression);

    }
}