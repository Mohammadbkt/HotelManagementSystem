using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Employees.Commands.DeleteEmployee
{
    public sealed record DeleteEmployeeCommand(
        Guid EmployeeId
    ) : IRequest<Result>;
}