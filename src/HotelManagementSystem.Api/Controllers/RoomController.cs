
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

        return result.Match(
            response => Ok(response),
            Problem);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] GetRoomsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            response => Ok(response),
            Problem);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoomCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            roomId => CreatedAtAction(
                nameof(GetById),
                new { id = roomId },
                roomId),
            Problem);
    }

    [HttpPut("{id:guid}/number")]
    public async Task<IActionResult> ChangeRoomNumber(
        Guid id,
        [FromBody] ChangeRoomNumberCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.RoomId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/category")]
    public async Task<IActionResult> ChangeRoomCategory(
        Guid id,
        [FromBody] ChangeRoomCategoryCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.RoomId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/clean")]
    public async Task<IActionResult> MarkAsClean(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new MarkRoomAsCleanCommand(id);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/maintenance")]
    public async Task<IActionResult> MoveToMaintenance(
        Guid id,
        [FromBody] MoveRoomToMaintenanceCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.RoomId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/service")]
    public async Task<IActionResult> ReturnToService(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ReturnRoomToServiceCommand(id);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/out-of-order")]
    public async Task<IActionResult> TakeOutOfOrder(
        Guid id,
        [FromBody] TakeRoomOutOfOrderCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.RoomId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors.ToList());
    }
}
