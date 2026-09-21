using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Hotels.Dtos;
using HotelManagementSystem.Application.Features.Hotels.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Hotels.Queries.GetHotelById
{
    public sealed class GetHotelByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetHotelByIdQuery, Result<HotelDto>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<HotelDto>> Handle(GetHotelByIdQuery request, CancellationToken cancellationToken)
        {
            var hotelDto = await _context.Hotels
                            .AsNoTracking()
                            .Where(h => h.Id == request.HotelId)
                            .ToHotelDto()
                            .FirstOrDefaultAsync(cancellationToken);

            if (hotelDto is null)
                return Result<HotelDto>.Failure(
                    ApplicationErrors.Hotel.NotFound);

            return Result<HotelDto>.Success(hotelDto);
        }
    }
}