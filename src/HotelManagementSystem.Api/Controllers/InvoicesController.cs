using HotelManagementSystem.Application.Features.Billing.Commands.AddInvoiceLineItem;
using HotelManagementSystem.Application.Features.Billing.Commands.ApplyInvoiceDiscount;
using HotelManagementSystem.Application.Features.Billing.Commands.ApplyInvoiceTax;
using HotelManagementSystem.Application.Features.Billing.Commands.CancelInvoice;
using HotelManagementSystem.Application.Features.Billing.Commands.CreateInvoice;
using HotelManagementSystem.Application.Features.Billing.Commands.IssueInvoice;
using HotelManagementSystem.Application.Features.Billing.Commands.PayInvoice;
using HotelManagementSystem.Application.Features.Billing.Queries.GetInvoiceById;
using HotelManagementSystem.Application.Features.Billing.Queries.GetInvoices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class InvoicesController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetInvoiceByIdQuery(id), cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Get([FromQuery] GetInvoicesQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.Match(id => CreatedAtAction(nameof(GetById), new { id }, new { InvoiceId = id }), Problem);
    }

    [HttpPost("{id:guid}/line-items")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> AddLineItem(Guid id, [FromBody] AddInvoiceLineItemCommand command, CancellationToken cancellationToken)
    {
        if (id != command.InvoiceId) return BadRequest();
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/discount")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ApplyDiscount(Guid id, [FromBody] ApplyInvoiceDiscountCommand command, CancellationToken cancellationToken)
    {
        if (id != command.InvoiceId) return BadRequest();
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }

    [HttpPut("{id:guid}/tax")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ApplyTax(Guid id, [FromBody] ApplyInvoiceTaxCommand command, CancellationToken cancellationToken)
    {
        if (id != command.InvoiceId) return BadRequest();
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }

    [HttpPost("{id:guid}/issue")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Issue(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new IssueInvoiceCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }

    [HttpPost("{id:guid}/pay")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Pay(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new PayInvoiceCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CancelInvoiceCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Errors.ToList());
    }
}
