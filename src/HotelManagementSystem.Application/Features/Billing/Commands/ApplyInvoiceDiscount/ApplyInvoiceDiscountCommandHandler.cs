using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Billing.Commands.ApplyInvoiceDiscount
{
    public sealed class ApplyInvoiceDiscountCommandHandler(IAppDbContext context, ILogger<ApplyInvoiceDiscountCommandHandler> logger) : IRequestHandler<ApplyInvoiceDiscountCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<ApplyInvoiceDiscountCommandHandler> _logger = logger;

        public async Task<Result> Handle(ApplyInvoiceDiscountCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
            if (invoice is null)
            {
                _logger.LogWarning("Cannot apply invoice discount: invoice {InvoiceId} was not found",
                            request.InvoiceId);

                return Result.Failure(ApplicationErrors.Invoice.NotFound);
            }
            var discountResult = Money.Create(request.DiscountAmount, request.DiscountCurrency);
            if (discountResult.IsError)
            {
                _logger.LogWarning("Cannot apply invoice discount to invoice {InvoiceId}: invalid discount amount or currency",
                request.InvoiceId);
                return Result.Failure(discountResult.Errors);
            }

            var result = invoice.ApplyDiscount(discountResult.Value);
            if (result.IsError)
            {
                _logger.LogWarning("Cannot apply invoice discount to invoice {InvoiceId}: {ErrorCode}",
                request.InvoiceId,
                result.Errors);
                return Result.Failure(result.Errors);
            }
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Applied discount to invoice {InvoiceId}: {DiscountAmount} {DiscountCurrency}",
                request.InvoiceId,
                request.DiscountAmount,
                request.DiscountCurrency);

            return Result.Success();
        }
    }
}