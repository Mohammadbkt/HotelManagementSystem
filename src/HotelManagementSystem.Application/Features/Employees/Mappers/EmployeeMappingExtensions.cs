using System.Linq.Expressions;
using HotelManagementSystem.Application.Features.Employees.Dtos;
using HotelManagementSystem.Domain.Entities.Employee;

namespace HotelManagementSystem.Application.Features.Employees.Mappers
{
    public static class EmployeeMappingExtensions
    {
        public static readonly Expression<Func<Employee, EmployeeDto>> ProjectionExpression =
        e => new EmployeeDto(
            e.Id,
            e.FirstName,
            e.LastName,
            e.EmailAddress.Value,
            e.PhoneNumber == null ? null : e.PhoneNumber.Value.Value,
            e.DateOfBirth,
            e.Role
        );

        private static readonly Func<Employee, EmployeeDto> _compiledProjection
            = ProjectionExpression.Compile();

        public static EmployeeDto ToDto(this Employee employee) => _compiledProjection(employee);


        public static IQueryable<EmployeeDto> ToEmployeeDto(this IQueryable<Employee> query)
            => query.Select(ProjectionExpression);
    }
}