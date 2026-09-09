using System.Linq.Expressions;
using HotelManagementSystem.Application.Features.RoomCategories.Dtos;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using Microsoft.EntityFrameworkCore.Query;

namespace HotelManagementSystem.Application.Features.RoomCategories.Mappers
{
    public static class RoomCategoryMappingExtensions
    {
        public static readonly Expression<Func<RoomCategory, RoomCategoryDto>> ProjectionExpression = 
            rc => new RoomCategoryDto(
                    rc.Id,
                    rc.HotelId,
                    rc.Name,
                    rc.Description,
                    rc.MaxCapacity,
                    rc.BasePricePerNight.Amount,
                    rc.BasePricePerNight.Currency);

            
            private static readonly Func<RoomCategory, RoomCategoryDto> _compiledProjection = ProjectionExpression.Compile();


            public static RoomCategoryDto ToDto(this RoomCategory roomCategory) => _compiledProjection(roomCategory);

            public static IQueryable<RoomCategoryDto> ToRoomCategoryDto(this IQueryable<RoomCategory> query)
                => query.Select(ProjectionExpression);
    }
}