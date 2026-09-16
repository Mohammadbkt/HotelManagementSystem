using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Employees.Dtos;
using HotelManagementSystem.Application.Features.Employees.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Employees.Queries.GetEmployees
{
    public sealed class GetEmployeesQueryHandler(IAppDbContext context) : IRequestHandler<GetEmployeesQuery, Result<PaginatedList<EmployeeDto>>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<PaginatedList<EmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Employees
                            .AsNoTracking()
                            .AsQueryable()
                            .Where(e => e.HotelId == request.HotelId);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(e =>
                    e.FirstName.Contains(term) ||
                    e.LastName.Contains(term));
            }

            if (request.Role.HasValue)
                query = query.Where(e => e.Role == request.Role);

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var term = request.PhoneNumber.Trim();
                query = query.Where(e =>
                    e.PhoneNumber.HasValue &&
                    e.PhoneNumber.Value.Value.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var term = request.Email.Trim();
                query = query.Where(e => e.EmailAddress.Value.Contains(term));
            }

            query = request.SortBy?.ToLowerInvariant() switch
            {
                _ => request.SortDirection == SortDirection.Ascending
                                ? query = query.OrderBy(e => e.FirstName)
                                : query = query.OrderByDescending(e => e.FirstName),

            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToEmployeeDto()
                        .ToListAsync(cancellationToken);

            var paginatedList = new PaginatedList<EmployeeDto>
            {
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Items = items
            };

            return Result<PaginatedList<EmployeeDto>>.Success(paginatedList);



        }
    }
}