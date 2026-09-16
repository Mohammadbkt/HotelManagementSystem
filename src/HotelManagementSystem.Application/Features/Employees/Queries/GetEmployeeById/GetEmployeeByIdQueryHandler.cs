using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Features.Employees.Dtos;
using HotelManagementSystem.Application.Features.Employees.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Employees.Queries.GetEmployeeById
{
    public sealed class GetEmployeeByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employeeDto = await _context.Employees
                                .AsNoTracking()
                                .Where(e => e.Id == request.EmployeeId)
                                .ToEmployeeDto()
                                .SingleOrDefaultAsync(cancellationToken);

            if(employeeDto is null)
                return Result<EmployeeDto>.Failure(Error.NotFound("", ""));

            return Result<EmployeeDto>.Success(employeeDto);
        }
    }
}