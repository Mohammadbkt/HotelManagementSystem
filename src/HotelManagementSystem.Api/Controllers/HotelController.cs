using HotelManagementSystem.Application.Features.Hotels.Commands.CreateHotel;
using HotelManagementSystem.Application.Features.Hotels.Commands.UpdateHotel;
using HotelManagementSystem.Application.Features.Hotels.Queries.GetHotelById;
using HotelManagementSystem.Application.Features.Hotels.Queries.GetHotels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class HotelController(ISender sender) : ApiController
    {
        private readonly ISender _sender = sender;

        [HttpGet("{id:Guid}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetHotelByIdQuery(id), cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(result.Errors.ToList());
        }


        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] GetHotelsQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return Problem(result.Errors.ToList());
        }


        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateHotelCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
            else
                return Problem(result.Errors.ToList());
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateHotelCommand command, CancellationToken cancellationToken)
        {

            if (id != command.HotelId)
                return Problem(
                    title: "Invalid request",
                    detail: "The route ID must match the hotel ID in the request body.",
                    statusCode: StatusCodes.Status400BadRequest);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsSuccess)
                return NoContent();
            else
                return Problem(result.Errors.ToList());

        }



    }
}