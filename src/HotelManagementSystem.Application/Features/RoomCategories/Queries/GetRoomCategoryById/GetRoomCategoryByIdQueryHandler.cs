using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Features.RoomCategories.Dtos;
using HotelManagementSystem.Application.Features.RoomCategories.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoryById
{
    public class GetRoomCategoryByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetRoomCategoryByIdQuery, Result<RoomCategoryDto>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<RoomCategoryDto>> Handle(GetRoomCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var roomCategoryDto = await _context.RoomCategories
                                .AsNoTracking()
                                .Where(rc => rc.Id == request.RoomCategoryId)
                                .ToRoomCategoryDto()
                                .FirstOrDefaultAsync(cancellationToken);

            if(roomCategoryDto is null)
                return Result<RoomCategoryDto>.Failure(Error.NotFound("RoomCategory.NotFound", "Room category not found"));

            return Result<RoomCategoryDto>.Success(roomCategoryDto);
        }
    }
}