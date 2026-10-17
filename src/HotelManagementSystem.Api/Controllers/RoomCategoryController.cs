
using HotelManagementSystem.Application.Features.RoomCategories.Commands.CreateRoomCategory;
using HotelManagementSystem.Application.Features.RoomCategories.Commands.DeactivateRoomCategory;
using HotelManagementSystem.Application.Features.RoomCategories.Commands.UpdateBasePriceForRoomCategory;
using HotelManagementSystem.Application.Features.RoomCategories.Commands.UpdateRoomCategory;
using HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoriesByHotel;
using HotelManagementSystem.Application.Features.RoomCategories.Queries.GetRoomCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RoomCategoryController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetRoomCategoryByIdQuery(id);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            response => Ok(response),
            Problem);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByHotel(
        [FromQuery] GetRoomCategoriesByHotelQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            response => Ok(response),
            Problem);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoomCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            id => CreatedAtAction(
                nameof(GetById),
                new { id },
                new { RoomCategoryId = id }),
            Problem);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateRoomCategoryCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.RoomCategoryId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/base-price")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> UpdateBasePrice(
        Guid id,
        [FromBody] UpdateRoomCategoryBasePriceCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.RoomCategoryId)
            return BadRequest();

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors.ToList());
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateRoomCategoryCommand(id);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(result.Errors.ToList());
    }
}
