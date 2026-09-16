using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Employees.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Employee;
using MediatR;

namespace HotelManagementSystem.Application.Features.Employees.Queries.GetEmployees
{
    public sealed record GetEmployeesQuery(
        Guid HotelId,
        string? SearchTerm = null,
        EmployeeRole? Role = null,
        string? PhoneNumber = null,
        string? Email = null,
        string? SortBy = null,
        SortDirection SortDirection = SortDirection.Ascending,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<Result<PaginatedList<EmployeeDto>>>;
}