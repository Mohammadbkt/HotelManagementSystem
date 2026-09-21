using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Employees.Commands.DeleteEmployee
{
    public sealed class DeleteEmployeeCommandHandler(IAppDbContext context)
        : IRequestHandler<DeleteEmployeeCommand, Result>
    {
        public async Task<Result> Handle(
            DeleteEmployeeCommand request,
            CancellationToken ct)
        {
            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, ct);

            if (employee is null)
                return Result.Failure(
                    ApplicationErrors.Employee.NotFound);

            employee.Delete(Guid.Empty);
            
            await context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}