using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Rooms.Dtos;
using HotelManagementSystem.Application.Features.Rooms.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Rooms.Queries.GetRoomById
{
    public sealed class GetRoomByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<RoomDto>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
        {
            var roomDto = await _context.Rooms
                            .AsNoTracking()
                            .Where(r => r.Id == request.RoomId)
                            .ToRoomDto()
                            .FirstOrDefaultAsync(cancellationToken);

            if (roomDto is null) 
                return Result<RoomDto>.Failure(Error.NotFound("Room.NotFound", "Room not found."));

            return Result<RoomDto>.Success(roomDto);


        }
    }
}