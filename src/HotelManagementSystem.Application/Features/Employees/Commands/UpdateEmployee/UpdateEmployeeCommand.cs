using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployee
{
    public sealed record UpdateEmployeeCommand(
        Guid EmployeeId,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth) : IRequest<Result>;
}