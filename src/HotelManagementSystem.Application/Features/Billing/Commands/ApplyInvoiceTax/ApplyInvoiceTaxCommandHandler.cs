using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.ApplyInvoiceTax
{
    public sealed class ApplyInvoiceTaxCommandHandler(IAppDbContext context) : IRequestHandler<ApplyInvoiceTaxCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(ApplyInvoiceTaxCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
            if (invoice is null)
                return Result.Failure(ApplicationErrors.Invoice.NotFound);

            var taxResult = Money.Create(request.TaxAmount, request.TaxCurrency);
            if (taxResult.IsError)
                return Result.Failure(taxResult.Errors);

            var result = invoice.ApplyTax(taxResult.Value);

            if (result.IsError)
                return Result.Failure(result.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }

}