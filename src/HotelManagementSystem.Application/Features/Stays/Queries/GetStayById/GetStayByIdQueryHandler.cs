using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Stays.Dtos;
using HotelManagementSystem.Application.Features.Stays.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Stays.Queries.GetStayById
{
    public sealed class GetStayByIdQueryHandler(IAppDbContext context, ILogger<GetStayByIdQueryHandler> logger) : IRequestHandler<GetStayByIdQuery, Result<StayDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetStayByIdQueryHandler> _logger = logger;

        public async Task<Result<StayDto>> Handle(GetStayByIdQuery request, CancellationToken cancellationToken)
        {
            var stayDto = await _context.Stays
                    .AsNoTracking()
                    .Where(s => s.Id == request.StayId)
                    .ToStayDto()
                    .SingleOrDefaultAsync( cancellationToken);

            if(stayDto is null)
            {
                _logger.LogWarning("Stay {StayId} not found", request.StayId);
                return Result<StayDto>.Failure(ApplicationErrors.Stay.NotFound);
            }

            _logger.LogInformation("Stay {StayId} retrieved", request.StayId);
            return Result<StayDto>.Success(stayDto);

        }
    }
}