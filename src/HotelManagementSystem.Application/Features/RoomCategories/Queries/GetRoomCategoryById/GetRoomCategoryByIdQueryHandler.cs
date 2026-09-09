using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Features.RoomCategories.Dtos;
using HotelManagementSystem.Application.Features.RoomCategories.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoryById
{
    public class GetRoomCategoryByIdCommandHandler(IAppDbContext context) : IRequestHandler<GetRoomCategoryByIdQuery, Result<RoomCategoryDto>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<RoomCategoryDto>> Handle(GetRoomCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var roomCategory = await _context.RoomCategories.FindAsync(request.RoomCategoryId, cancellationToken);
            if(roomCategory is null)
                return Result<RoomCategoryDto>.Failure(Error.NotFound("RoomCategory.NotFound", "Room category not found"));

            var roomCategoryDto = roomCategory.ToDto();

            return Result<RoomCategoryDto>.Success(roomCategoryDto);
        }
    }
}