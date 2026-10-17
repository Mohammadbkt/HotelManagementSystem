using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Employees.Commands.DeleteEmployee
{
    public sealed class DeleteEmployeeCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        IIdentityService identity,
        ILogger<DeleteEmployeeCommandHandler> logger
        )
        : IRequestHandler<DeleteEmployeeCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ICurrentUserService _currentUser = currentUser;
        private readonly IIdentityService _identity = identity;        
        private readonly ILogger<DeleteEmployeeCommandHandler> _logger = logger;


        public async Task<Result> Handle(
            DeleteEmployeeCommand request,
            CancellationToken cancellationToken)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee is null)
            {
                _logger.LogWarning(
                    "Employee {EmployeeId} not found; cannot delete",
                    request.EmployeeId);

                return Result.Failure(ApplicationErrors.Employee.NotFound);
            }

            if (employee.UserId == _currentUser.UserId)
                return Result.Failure(ApplicationErrors.Employee.CannotDeleteSelf);

            employee.Delete(_currentUser.UserId ?? Guid.Empty);

            var disable = await _identity.DeleteUserAsync(employee.UserId, cancellationToken); // or DeleteUserAsync
            if (disable.IsError) return Result.Failure(disable.Errors);

            await _context.RefreshTokens
                .Where(t => t.UserId == employee.UserId && t.RevokedAt == null)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, DateTimeOffset.UtcNow), cancellationToken);


            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Employee {EmployeeId} deleted by user {ActorId}",
                request.EmployeeId,
                _currentUser.UserId);

            return Result.Success();
        }
    }
}