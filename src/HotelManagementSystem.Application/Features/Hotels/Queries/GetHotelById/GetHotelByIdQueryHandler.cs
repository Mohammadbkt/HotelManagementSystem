using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Hotels.Dtos;
using HotelManagementSystem.Application.Features.Hotels.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Hotels.Queries.GetHotelById
{
    public sealed class GetHotelByIdQueryHandler(IAppDbContext context, ILogger<GetHotelByIdQueryHandler> logger) : IRequestHandler<GetHotelByIdQuery, Result<HotelDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetHotelByIdQueryHandler> _logger = logger;
        public async Task<Result<HotelDto>> Handle(GetHotelByIdQuery request, CancellationToken cancellationToken)
        {
            var hotelDto = await _context.Hotels
                            .AsNoTracking()
                            .Where(h => h.Id == request.HotelId)
                            .ToHotelDto()
                            .FirstOrDefaultAsync(cancellationToken);

            if (hotelDto is null)
            {
                _logger.LogWarning("Hotel {HotelId} not found", request.HotelId);
                return Result<HotelDto>.Failure(
                    ApplicationErrors.Hotel.NotFound);
            }

            _logger.LogInformation("Hotel {HotelId} retrieved", request.HotelId);
            return Result<HotelDto>.Success(hotelDto);
        }
    }
}