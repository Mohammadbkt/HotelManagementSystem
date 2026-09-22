using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Billing.Commands.AddInvoiceLineItem
{
    public sealed class AddInvoiceLineItemCommandHandler(IAppDbContext context, ILogger<AddInvoiceLineItemCommandHandler> logger) : IRequestHandler<AddInvoiceLineItemCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<AddInvoiceLineItemCommandHandler> _logger = logger;

        public async Task<Result> Handle(AddInvoiceLineItemCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
            if (invoice is null)
            {
                _logger.LogWarning("Cannot add line item: invoice {InvoiceId} was not found", request.InvoiceId);

                return Result.Failure(ApplicationErrors.Room.NotFound);
            }

            var amountResult = Money.Create(request.UnitPriceAmount, request.UnitPriceCurrency);
            if (amountResult.IsError)
                return Result.Failure(amountResult.Errors);

            var result = invoice.AddLineItem(request.Description, request.Quantity, amountResult.Value);
            if (result.IsError)
            {
                _logger.LogWarning("Cannot add line item to invoice {InvoiceId}",
                                    request.InvoiceId);

                return Result.Failure(result.Errors);
            }
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Invoice line item added to invoice {InvoiceId}",
                request.InvoiceId);

            return Result.Success();
        }
    }
}