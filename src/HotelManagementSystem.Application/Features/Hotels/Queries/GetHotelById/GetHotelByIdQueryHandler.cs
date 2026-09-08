using HotelManagementSystem.Application.Common;
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
            var hotel = await _context.Hotels.SingleOrDefaultAsync(x => x.Id == request.HotelId, cancellationToken);
            if(hotel is null)
                return Result<HotelDto>.Failure(Error.NotFound("Hotel.NotFound", "Hotel not found."));

            var hotelDto = hotel.ToDto();

            return Result<HotelDto>.Success(hotelDto);
        }
    }
}