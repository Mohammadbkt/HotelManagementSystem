using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployeeContactInfo
{

    public sealed record UpdateEmployeeContactInfoCommand(
        Guid EmployeeId,
        string Email,
        string? PhoneNumber) : IRequest<Result>;

}