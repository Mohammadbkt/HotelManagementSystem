using HotelManagementSystem.Application.Features.Stays.Commands.CheckInGuest;
using HotelManagementSystem.Application.Features.Stays.Commands.CheckOutGuest;
using HotelManagementSystem.Application.Features.Stays.Queries.GetStayById;
using HotelManagementSystem.Application.Features.Stays.Queries.GetStays;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class StaysController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetStayByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);
        else
            return Problem(result.Errors.ToList());
    }

    [HttpGet]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Get([FromQuery] GetStaysQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);
        else
            return Problem(result.Errors.ToList());
    }

    [HttpPost("check-in")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInGuestCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { stayId = result.Value }, new { result.Value });
            else
            return Problem(result.Errors.ToList());

    }

    [HttpPost("{id:guid}/check-out")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> CheckOut(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CheckOutGuestCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }
}
