using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Guests.Dtos;
using HotelManagementSystem.Application.Features.Guests.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Guests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Guests.Queries.GetGuestById
{
    public sealed class GetGuestByIdQueryHandler(IAppDbContext context, ILogger<GetGuestByIdQueryHandler> logger) : IRequestHandler<GetGuestByIdQuery, Result<GuestDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetGuestByIdQueryHandler> _logger = logger;

        public async Task<Result<GuestDto>> Handle(GetGuestByIdQuery request, CancellationToken cancellationToken)
        {

            var guestDto = await _context.Guests
                            .AsNoTracking()
                            .Where(g => g.Id == request.GuestId)
                            .ToGuestDto()
                            .FirstOrDefaultAsync(cancellationToken);

            if(guestDto is null)
            {
                _logger.LogWarning("Guest {GuestId} not found", request.GuestId);
                return Result<GuestDto>.Failure(GuestErrors.NotFound);
            }

            _logger.LogInformation("Guest {GuestId} retrieved", request.GuestId);
            return Result<GuestDto>.Success(guestDto);
        }
    }
}