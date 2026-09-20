using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Stays.Dtos;
using HotelManagementSystem.Application.Features.Stays.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Stays.Queries.GetStayById
{
    public sealed class GetStayByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetStayByIdQuery, Result<StayDto>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<StayDto>> Handle(GetStayByIdQuery request, CancellationToken cancellationToken)
        {
            var stayDto = await _context.Stays
                    .AsNoTracking()
                    .Where(s => s.Id == request.StayId)
                    .ToStayDto()
                    .SingleOrDefaultAsync( cancellationToken);

            if(stayDto is null)
                return Result<StayDto>.Failure(Error.NotFound("Stay.NotFound", "Stay not found."));

            return Result<StayDto>.Success(stayDto);

        }
    }
}