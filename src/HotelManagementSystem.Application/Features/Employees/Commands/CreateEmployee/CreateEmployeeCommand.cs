using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Employee;
using MediatR;

namespace HotelManagementSystem.Application.Features.Employees.Commands.CreateEmployee
{
    public sealed record CreateEmployeeCommand(
        Guid HotelId,
        string FirstName,
        string LastName,
        string? PhoneNumber,
        string? Email,
        DateOnly DateOfBirth,
        EmployeeRole Role
    ) : IRequest<Result<Guid>>;
}