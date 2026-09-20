using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.CancelInvoice
{
    public sealed class CancelInvoiceCommandHandler(IAppDbContext context) : IRequestHandler<CancelInvoiceCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
            if(invoice is null)
                return Result.Failure(Error.NotFound());

            var invoiceResult = invoice.Cancel();
            if(invoiceResult.IsError)
                return  Result.Failure(invoiceResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}