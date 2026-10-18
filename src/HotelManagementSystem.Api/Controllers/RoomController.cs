
using HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomCategory;
using HotelManagementSystem.Application.Features.Rooms.Commands.ChangeRoomNumber;
using HotelManagementSystem.Application.Features.Rooms.Commands.CreateRoom;
using HotelManagementSystem.Application.Features.Rooms.Commands.MarkRoomAsClean;
using HotelManagementSystem.Application.Features.Rooms.Commands.MoveRoomToMaintenance;
using HotelManagementSystem.Application.Features.Rooms.Commands.ReturnRoomToService;
using HotelManagementSystem.Application.Features.Rooms.Commands.TakeRoomOutOfOrder;
using HotelManagementSystem.Application.Features.Rooms.Queries.GetRoomById;
using HotelManagementSystem.Application.Features.Rooms.Queries.GetRooms;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RoomController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetRoomByIdQuery(id);

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);
        else
            return Problem(result.Errors.ToList());
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] GetRoomsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        
        if (result.IsSuccess)
            return Ok(result.Value);
        else
            return Problem(result.Errors.ToList());
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoomCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { roomId = result.Value }, result.Value);
        else
            return Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/number")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ChangeRoomNumber(
        Guid id,
        [FromBody] ChangeRoomNumberCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.RoomId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        else
            return Problem(result.Errors.ToList());

    }

    [HttpPut("{id:guid}/category")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ChangeRoomCategory(
        Guid id,
        [FromBody] ChangeRoomCategoryCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.RoomId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        else
            return Problem(result.Errors.ToList());

    }

    [HttpPut("{id:guid}/clean")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> MarkAsClean(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new MarkRoomAsCleanCommand(id);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        else
            return Problem(result.Errors.ToList());

    }

    [HttpPut("{id:guid}/maintenance")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> MoveToMaintenance(
        Guid id,
        [FromBody] MoveRoomToMaintenanceCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.RoomId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        else
            return Problem(result.Errors.ToList());

    }

    [HttpPut("{id:guid}/service")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ReturnToService(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ReturnRoomToServiceCommand(id);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        else
            return Problem(result.Errors.ToList());

    }

    [HttpPut("{id:guid}/out-of-order")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> TakeOutOfOrder(
        Guid id,
        [FromBody] TakeRoomOutOfOrderCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.RoomId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsSuccess)
            return NoContent();
        else
            return Problem(result.Errors.ToList());

    }
}
