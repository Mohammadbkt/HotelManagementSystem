using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Employee;
using MediatR;

namespace HotelManagementSystem.Application.Features.Employees.Commands.ChangeEmployeeRole
{
    public sealed record ChangeEmployeeRoleCommand(
        Guid EmployeeId,
        EmployeeRole NewRole
    ) : IRequest<Result>;
}