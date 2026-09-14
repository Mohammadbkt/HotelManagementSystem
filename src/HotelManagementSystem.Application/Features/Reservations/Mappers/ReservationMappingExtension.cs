using System.Linq.Expressions;
using HotelManagementSystem.Application.Features.Reservations.Dtos;
using HotelManagementSystem.Domain.Entities.Reservations;

namespace HotelManagementSystem.Application.Features.Reservations.Mappers
{
    public static class ReservationMappingExtension
    {
        public static Expression<Func<Reservation, ReservationDto>> ProjectionExpression =
        r => new ReservationDto(
            r.Id,
            r.RoomId,
            r.Status,
            r.CheckInDate,
            r.CheckOutDate
            );


        private static readonly Func<Reservation, ReservationDto> _compiledProjection = ProjectionExpression.Compile();

        public static ReservationDto ToDto(this Reservation reservation)
            => _compiledProjection(reservation);

        public static IQueryable<ReservationDto> ToReservationDto(this IQueryable<Reservation> query)
            => query.Select(ProjectionExpression);
    }
}