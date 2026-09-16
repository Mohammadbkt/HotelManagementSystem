using HotelManagementSystem.Application.Features.Employees.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Employees.Queries.GetEmployeeById
{
    public sealed record GetEmployeeByIdQuery(
        Guid EmployeeId
    ) : IRequest<Result<EmployeeDto>>;
}