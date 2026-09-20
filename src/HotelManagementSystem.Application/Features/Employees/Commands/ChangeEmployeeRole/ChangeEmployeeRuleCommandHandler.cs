using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Employees.Commands.ChangeEmployeeRole
{
    public sealed class ChangeEmployeeRoleCommandHandler(IAppDbContext context)
        : IRequestHandler<ChangeEmployeeRoleCommand, Result>
    {
        public async Task<Result> Handle(
            ChangeEmployeeRoleCommand request,
            CancellationToken ct)
        {
            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, ct);

            if (employee is null)
                return Result.Failure(
                    Error.NotFound(
                        "Employee.NotFound",
                        $"Employee {request.EmployeeId} was not found."));

            var roleResult = employee.ChangeRole(request.NewRole);
            if (roleResult.IsError)
                return roleResult;

            await context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}