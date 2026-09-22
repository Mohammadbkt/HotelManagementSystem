using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Rooms.Dtos;
using HotelManagementSystem.Application.Features.Rooms.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Rooms.Queries.GetRoomById
{
    public sealed class GetRoomByIdQueryHandler(IAppDbContext context, ILogger<GetRoomByIdQueryHandler> logger) : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetRoomByIdQueryHandler> _logger = logger;

        public async Task<Result<RoomDto>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
        {
            var roomDto = await _context.Rooms
                            .AsNoTracking()
                            .Where(r => r.Id == request.RoomId)
                            .ToRoomDto()
                            .FirstOrDefaultAsync(cancellationToken);

            if (roomDto is null) 
            {
                _logger.LogWarning("Room {RoomId} not found", request.RoomId);
                return Result<RoomDto>.Failure(ApplicationErrors.NotFound);
            }

            _logger.LogInformation("Room {RoomId} retrieved", request.RoomId);
            return Result<RoomDto>.Success(roomDto);


        }
    }
}