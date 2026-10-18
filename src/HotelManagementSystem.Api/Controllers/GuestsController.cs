using HotelManagementSystem.Application.Features.Guests.Commands.CreateGuest;
using HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuest;
using HotelManagementSystem.Application.Features.Guests.Commands.UpdateGuestContactInfo;
using HotelManagementSystem.Application.Features.Guests.Queries.GetGuestById;
using HotelManagementSystem.Application.Features.Guests.Queries.GetGuests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class GuestsController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetGuestByIdQuery(id), cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        else
            return Problem(result.Errors.ToList());
    }

    [HttpGet]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get([FromQuery] GetGuestsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        else
            return Problem(result.Errors.ToList());
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateGuestCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        else
            return Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGuestInfoCommand command, CancellationToken cancellationToken)
    {
        if (id != command.GuestId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        else
            return Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/contact-info")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateContactInfo(Guid id, [FromBody] UpdateGuestContactInfoCommand command, CancellationToken cancellationToken)
    {
        if (id != command.GuestId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        else
            return Problem(result.Errors.ToList());
    }
}
