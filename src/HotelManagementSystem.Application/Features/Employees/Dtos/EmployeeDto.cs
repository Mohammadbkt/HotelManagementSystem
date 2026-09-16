using HotelManagementSystem.Domain.Entities.Employee;

namespace HotelManagementSystem.Application.Features.Employees.Dtos
{
    public sealed record EmployeeDto(
        Guid Id,
        string FirstName,
        string LastName,
        string? Email,
        string? PhoneNumber,
        DateOnly DateOfBirth,
        EmployeeRole Role

    );
}