using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.RoomCategories.Dtos;
using HotelManagementSystem.Application.Features.RoomCategories.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoryById
{
    public class GetRoomCategoryByIdQueryHandler(IAppDbContext context, ILogger<GetRoomCategoryByIdQueryHandler> logger) : IRequestHandler<GetRoomCategoryByIdQuery, Result<RoomCategoryDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetRoomCategoryByIdQueryHandler> _logger = logger;
        public async Task<Result<RoomCategoryDto>> Handle(GetRoomCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var roomCategoryDto = await _context.RoomCategories
                                .AsNoTracking()
                                .Where(rc => rc.Id == request.RoomCategoryId)
                                .ToRoomCategoryDto()
                                .FirstOrDefaultAsync(cancellationToken);

            if(roomCategoryDto is null)
            {
                _logger.LogWarning("Room category {RoomCategoryId} not found", request.RoomCategoryId);
                return Result<RoomCategoryDto>.Failure(ApplicationErrors.RoomCategory.NotFound);
            }

            _logger.LogInformation("Room category {RoomCategoryId} retrieved", request.RoomCategoryId);
            return Result<RoomCategoryDto>.Success(roomCategoryDto);
        }
    }
}