using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Employees.Commands.DeleteEmployee;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployee
{
    public sealed class UpdateEmployeeCommandHandler(IAppDbContext context, ILogger<UpdateEmployeeCommandHandler> logger)
        : IRequestHandler<UpdateEmployeeCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateEmployeeCommandHandler> _logger = logger;

        public async Task<Result> Handle(
            UpdateEmployeeCommand request,
            CancellationToken cancellationToken)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee is null)
            {
                _logger.LogWarning(
                    "Employee {EmployeeId} not found; cannot update employee info",
                    request.EmployeeId);

                return Result.Failure(ApplicationErrors.Employee.NotFound);
            }
            var nameResult = employee.UpdateInfo(request.FirstName, request.LastName, request.DateOfBirth);
            if (nameResult.IsError)
                return nameResult;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Employee {EmployeeId} updated",
                request.EmployeeId);

            return Result.Success();
        }
    }
}