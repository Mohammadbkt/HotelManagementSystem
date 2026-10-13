using HotelManagementSystem.Application.Features.Employees.Commands.ChangeEmployeeRole;
using HotelManagementSystem.Application.Features.Employees.Commands.CreateEmployee;
using HotelManagementSystem.Application.Features.Employees.Commands.DeleteEmployee;
using HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployee;
using HotelManagementSystem.Application.Features.Employees.Commands.UpdateEmployeeContactInfo;
using HotelManagementSystem.Application.Features.Employees.Queries.GetEmployeeById;
using HotelManagementSystem.Application.Features.Employees.Queries.GetEmployees;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class EmployeesController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEmployeeByIdQuery(id), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetEmployeesQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.Match(id => CreatedAtAction(nameof(GetById), new { id }, new { EmployeeId = id }), Problem);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeCommand command, CancellationToken cancellationToken)
    {
        if (id != command.EmployeeId) return BadRequest();
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/contact-info")]
    public async Task<IActionResult> UpdateContactInfo(Guid id, [FromBody] UpdateEmployeeContactInfoCommand command, CancellationToken cancellationToken)
    {
        if (id != command.EmployeeId) return BadRequest();
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/role")]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeEmployeeRoleCommand command, CancellationToken cancellationToken)
    {
        if (id != command.EmployeeId) return BadRequest();
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteEmployeeCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }
}
