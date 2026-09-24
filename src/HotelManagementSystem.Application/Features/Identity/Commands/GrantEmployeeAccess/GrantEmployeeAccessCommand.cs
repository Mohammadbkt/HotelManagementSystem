using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Employee;
using MediatR;

namespace HotelManagementSystem.Application.Features.Identity.Commands.GrantEmployeeAccess;

public sealed record GrantEmployeeAccessCommand(
    Guid EmployeeId,
    string TemporaryPassword,
    EmployeeRole EmployeeRole,
    string Username
) : IRequest<Result>;