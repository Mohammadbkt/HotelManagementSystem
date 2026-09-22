using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Billing.Commands.PayInvoice
{
    public sealed class PayInvoiceCommandHandler(IAppDbContext context, ILogger<PayInvoiceCommandHandler> logger) : IRequestHandler<PayInvoiceCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<PayInvoiceCommandHandler> _logger = logger;

        public async Task<Result> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
            if (invoice is null)
            {
                _logger.LogWarning("Invoice {InvoiceId} not found", request.InvoiceId);

                return Result.Failure(ApplicationErrors.Invoice.NotFound);
            }
            var invoiceResult = invoice.Pay(DateTimeOffset.UtcNow);
            if (invoiceResult.IsError)
            {
                _logger.LogWarning(
                    "Failed to pay invoice {InvoiceId}: {Errors}",
                    request.InvoiceId,
                    invoiceResult.Errors);

                return Result.Failure(invoiceResult.Errors);
            }
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Invoice {InvoiceId} paid successfully", request.InvoiceId);

            return Result.Success();
        }
    }
}