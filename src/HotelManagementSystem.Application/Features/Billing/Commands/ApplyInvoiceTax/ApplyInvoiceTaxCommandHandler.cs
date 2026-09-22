using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Billing.Commands.ApplyInvoiceTax
{
    public sealed class ApplyInvoiceTaxCommandHandler(IAppDbContext context, ILogger<ApplyInvoiceTaxCommandHandler> logger) : IRequestHandler<ApplyInvoiceTaxCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<ApplyInvoiceTaxCommandHandler> _logger = logger;
        public async Task<Result> Handle(ApplyInvoiceTaxCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
            if (invoice is null)
            {
                _logger.LogWarning(
                "Cannot apply invoice tax: invoice {InvoiceId} was not found",
                request.InvoiceId);

                return Result.Failure(ApplicationErrors.Invoice.NotFound);
            }
            var taxResult = Money.Create(request.TaxAmount, request.TaxCurrency);
            if (taxResult.IsError)
            {
                return Result.Failure(taxResult.Errors);
            }
            var result = invoice.ApplyTax(taxResult.Value);

            if (result.IsError)
            {
                return Result.Failure(result.Errors);
            }
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                        "Invoice tax applied to invoice {InvoiceId}: {TaxAmount} {TaxCurrency}",
                        request.InvoiceId,
                        request.TaxAmount,
                        request.TaxCurrency);
                        
            return Result.Success();
        }
    }

}