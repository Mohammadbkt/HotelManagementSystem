using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Employee;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Identity.Commands.GrantEmployeeAccess
{
    public sealed class GrantEmployeeAccessCommandHandler(
     IAppDbContext context,
     IIdentityService identity,
     ILogger<GrantEmployeeAccessCommandHandler> logger ) 
     : IRequestHandler<GrantEmployeeAccessCommand, Result>
    {
        public async Task<Result> Handle(GrantEmployeeAccessCommand request, CancellationToken cancellationToken)
        {
            var employee = await context.Employees.FindAsync([request.EmployeeId], cancellationToken);
            if (employee is null)
            {
                logger.LogWarning("Employee {EmployeeId} not found", request.EmployeeId);
                return Result.Failure(ApplicationErrors.Employee.NotFound);
            }

            if (employee.UserId is not null)
            {
                logger.LogWarning("Employee {EmployeeId} already has an account", request.EmployeeId);
                return Result.Failure(EmployeeErrors.AlreadyLinkedToAccount);
            }

            var createUserResult = await identity.CreateUserAsync(
                 employee.EmailAddress.Value, 
                 request.Username,
                 request.TemporaryPassword, 
                 null,
                 null,
                 null,
                 null,
                 cancellationToken);

            if (createUserResult.IsError)
            {
                logger.LogWarning("Account creation failed for employee {EmployeeId}: {Error}",
                    request.EmployeeId, createUserResult.TopError.Description);
                return Result.Failure(createUserResult.Errors);
            }

            var linkResult = employee.LinkToUserAccount(createUserResult.Value.Id);
            if (linkResult.IsError)
                return linkResult;

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Access granted to employee {EmployeeId}", request.EmployeeId);
            return Result.Success();
        }
    }
}