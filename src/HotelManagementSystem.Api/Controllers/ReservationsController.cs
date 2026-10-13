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
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetReservationByIdQuery(id), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetReservationsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationsCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.Match(id => CreatedAtAction(nameof(GetById), new { id }, new { ReservationId = id }), Problem);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReservationCommand command, CancellationToken cancellationToken)
    {
        if (id != command.ReservationId) return BadRequest();
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ConfirmReservationCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CancelReservationCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }
}
