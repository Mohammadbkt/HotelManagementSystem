using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Employees.Commands.ChangeEmployeeRole
{
    public sealed class ChangeEmployeeRoleCommandHandler(
        IAppDbContext context,
        ILogger<ChangeEmployeeRoleCommandHandler> logger,
        IIdentityService identityService)
        : IRequestHandler<ChangeEmployeeRoleCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<ChangeEmployeeRoleCommandHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;

        public async Task<Result> Handle(
            ChangeEmployeeRoleCommand request,
            CancellationToken cancellationToken)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee is null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found", request.EmployeeId);

                return Result.Failure(ApplicationErrors.Employee.NotFound);
            }
            var roleResult = employee.ChangeRole(request.NewRole);
            if (roleResult.IsError)
            {
                _logger.LogWarning("Failed to change role for employee {EmployeeId}: {Errors}",
                    request.EmployeeId,
                    roleResult.Errors);

                return Result.Failure(roleResult.Errors);
            }

            if (employee.UserId is { } userId)
            {
                var r = await _identityService.SetRoleAsync(userId, request.NewRole.ToString(), cancellationToken);
                if (r.IsError) return Result.Failure(r.Errors);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
               "Employee {EmployeeId} role changed to {NewRole} successfully",
               request.EmployeeId,
               request.NewRole);
            return Result.Success();
        }
    }
}