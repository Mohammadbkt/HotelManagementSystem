using System.Linq.Expressions;
using HotelManagementSystem.Application.Features.Rooms.Dtos;
using HotelManagementSystem.Domain.Entities.Rooms;

namespace HotelManagementSystem.Application.Features.Rooms.Mappers
{
    public static class RoomMappingExtensions
    {
        public static Expression<Func<Room, RoomDto>> ProjectionExpression =
        r => new RoomDto(
            r.Id,
            r.RoomNumber,
            r.HotelId,
            r.RoomCategoryId,
            r.Status,
            r.StatusReason
        );

        private static readonly Func<Room, RoomDto> _compiledProjection = ProjectionExpression.Compile();

        public static RoomDto ToDto(this Room room) => _compiledProjection(room);

        public static IQueryable<RoomDto> ToRoomDto(this IQueryable<Room> query)
            => query.Select(ProjectionExpression);
            
        }

}