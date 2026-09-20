using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Guests.Dtos;
using HotelManagementSystem.Application.Features.Guests.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Guests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Guests.Queries.GetGuestById
{
    public sealed class GetGuestByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetGuestByIdQuery, Result<GuestDto>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<GuestDto>> Handle(GetGuestByIdQuery request, CancellationToken cancellationToken)
        {
            var guestDto = await _context.Guests
                            .AsNoTracking()
                            .Where(g => g.Id == request.GuestId)
                            .ToGuestDto()
                            .FirstOrDefaultAsync(cancellationToken);

            if(guestDto is null)
                return Result<GuestDto>.Failure(GuestErrors.NotFound);

            return Result<GuestDto>.Success(guestDto);
        }
    }
}