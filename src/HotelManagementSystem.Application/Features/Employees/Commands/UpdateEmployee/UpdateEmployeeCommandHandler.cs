using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployee
{
    public sealed class UpdateEmployeeCommandHandler(IAppDbContext context)
        : IRequestHandler<UpdateEmployeeCommand, Result>
    {
        public async Task<Result> Handle(
            UpdateEmployeeCommand request,
            CancellationToken ct)
        {
            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, ct);

            if (employee is null)
                return Result.Failure(
                    Error.NotFound(
                        "Employee.NotFound",
                        $"Employee {request.EmployeeId} was not found."));

            var nameResult = employee.UpdateInfo(request.FirstName, request.LastName, request.DateOfBirth);
            if (nameResult.IsError)
                return nameResult;

            await context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}