using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Employee;
using MediatR;

namespace HotelManagementSystem.Application.Features.Employees.Commands.CreateEmployee
{
    public sealed record CreateEmployeeCommand(
        Guid HotelId,
        Guid UserId,
        string FirstName,
        string LastName,
        string? PhoneNumber,
        DateOnly DateOfBirth,
        EmployeeRole Role
    ) : IRequest<Result<Guid>>;
}