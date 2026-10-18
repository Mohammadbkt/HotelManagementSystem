using HotelManagementSystem.Application.Features.Reservations.Commands.CancelReservation;
using HotelManagementSystem.Application.Features.Reservations.Commands.ConfirmReservation;
using HotelManagementSystem.Application.Features.Reservations.Commands.CreateReservations;
using HotelManagementSystem.Application.Features.Reservations.Commands.UpdateReservation;
using HotelManagementSystem.Application.Features.Reservations.Queries.GetReservationById;
using HotelManagementSystem.Application.Features.Reservations.Queries.GetReservations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ReservationsController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetReservationByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);
        else
            return Problem(result.Errors.ToList());
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]

    public async Task<IActionResult> Get([FromQuery] GetReservationsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);
        else
            return Problem(result.Errors.ToList());
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]

    public async Task<IActionResult> Create([FromBody] CreateReservationsCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { ReservationId = result.Value }, result.Value);
        else
            return Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]

    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReservationCommand command, CancellationToken cancellationToken)
    {
        if (id != command.ReservationId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        else
            return Problem(result.Errors.ToList());
    }

    [HttpPost("{id:guid}/confirm")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ConfirmReservationCommand(id), cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        else
            return Problem(result.Errors.ToList());
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CancelReservationCommand(id), cancellationToken);
       
        if (result.IsSuccess)
            return NoContent();
        else
            return Problem(result.Errors.ToList());
    }
}
