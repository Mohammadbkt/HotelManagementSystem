using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Employees.Dtos;
using HotelManagementSystem.Application.Features.Employees.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Employees.Queries.GetEmployeeById
{
    public sealed class GetEmployeeByIdQueryHandler(IAppDbContext context, ILogger<GetEmployeeByIdQueryHandler> logger) : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetEmployeeByIdQueryHandler> _logger = logger;

        public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting employee {EmployeeId}", request.EmployeeId);

            var employeeDto = await _context.Employees
                                .AsNoTracking()
                                .Where(e => e.Id == request.EmployeeId)
                                .ToEmployeeDto()
                                .SingleOrDefaultAsync(cancellationToken);

            if(employeeDto is null)
            {
                return Result<EmployeeDto>.Failure(ApplicationErrors.Employee.NotFound);
            }

            return Result<EmployeeDto>.Success(employeeDto);
        }
    }
}