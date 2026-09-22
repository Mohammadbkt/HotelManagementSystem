using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Billing.Commands.CancelInvoice
{
    public sealed class CancelInvoiceCommandHandler(IAppDbContext context, ILogger<CancelInvoiceCommandHandler> logger) : IRequestHandler<CancelInvoiceCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<CancelInvoiceCommandHandler> _logger = logger;
        public async Task<Result> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
            if (invoice is null)
            {
                _logger.LogWarning("Cannot cancel invoice: invoice {InvoiceId} was not found", request.InvoiceId);

                return Result.Failure(ApplicationErrors.Invoice.NotFound);
            }
            var invoiceResult = invoice.Cancel();
            if (invoiceResult.IsError)
            {
                _logger.LogWarning(
                "Cannot cancel invoice {InvoiceId}: {ErrorCode}",
                request.InvoiceId,
                invoiceResult.Errors);
                return Result.Failure(invoiceResult.Errors);
            }
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Invoice {InvoiceId} cancelled",
                request.InvoiceId);

            return Result.Success();

        }
    }
}