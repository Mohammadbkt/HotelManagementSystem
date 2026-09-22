using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Guests.Dtos;
using HotelManagementSystem.Application.Features.Guests.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Guests.Queries.GetGuests
{
    public sealed class GetGuestsQueryHandler(IAppDbContext context, ILogger<GetGuestsQueryHandler> logger) : IRequestHandler<GetGuestsQuery, Result<PaginatedList<GuestDto>>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetGuestsQueryHandler> _logger = logger;

        public async Task<Result<PaginatedList<GuestDto>>> Handle(GetGuestsQuery request, CancellationToken cancellationToken)
        {

            var query = _context.Guests
                        .AsNoTracking()
                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query
                        .Where(g => g.FirstName.Contains(request.SearchTerm)
                            || g.LastName.Contains(request.SearchTerm)
                            || (g.PhoneNumber != null && g.PhoneNumber.Value.Value.Contains(request.SearchTerm))
                            || (g.Email != null && g.Email.Value.Value.Contains(request.SearchTerm)));


            query = request.SortBy?.ToLowerInvariant() switch
            {
                "firstname" => request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(g => g.FirstName)
                    : query.OrderByDescending(g => g.FirstName),

                "lastname" => request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(g => g.LastName)
                    : query.OrderByDescending(g => g.LastName),

                "email" => request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(g => g.Email!.Value)
                    : query.OrderByDescending(g => g.Email!.Value),

                _ => request.SortDirection == SortDirection.Ascending
                    ? query.OrderBy(g => g.FirstName)
                    : query.OrderByDescending(g => g.FirstName)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToGuestDto()
                        .ToListAsync(cancellationToken);

            
            var paginatedResult = new PaginatedList<GuestDto>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Items = items
            };

            _logger.LogInformation("Fetched {TotalCount} guests (page {PageNumber}/{TotalPages})", totalCount, request.PageNumber, paginatedResult.TotalPages);
            return Result<PaginatedList<GuestDto>>.Success(paginatedResult);

            
        }
    }
}