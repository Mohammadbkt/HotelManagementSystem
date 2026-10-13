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
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetGuestByIdQuery(id), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetGuestsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGuestCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.Match(id => CreatedAtAction(nameof(GetById), new { id }, new { GuestId = id }), Problem);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGuestInfoCommand command, CancellationToken cancellationToken)
    {
        if (id != command.GuestId) return BadRequest();
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/contact-info")]
    public async Task<IActionResult> UpdateContactInfo(Guid id, [FromBody] UpdateGuestContactInfoCommand command, CancellationToken cancellationToken)
    {
        if (id != command.GuestId) return BadRequest();
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }
}
