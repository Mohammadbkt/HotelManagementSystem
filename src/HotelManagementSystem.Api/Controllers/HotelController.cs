using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Application.Features.Hotels.Commands.CreateHotel;
using HotelManagementSystem.Application.Features.Hotels.Commands.UpdateHotel;
using HotelManagementSystem.Application.Features.Hotels.Queries.GetHotelById;
using HotelManagementSystem.Application.Features.Hotels.Queries.GetHotels;
using HotelManagementSystem.Domain.Entities.Employee;
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

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(GetHotelByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem
                );
        }


        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] GetHotelsQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }


        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([FromBody] CreateHotelCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.Match(
                response => Ok(response),
                Problem);
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHotelCommand command, CancellationToken cancellationToken)
        {

            if (id != command.HotelId) return BadRequest();
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess ? Ok() : Problem(result.Errors.ToList());
        }



    }
}