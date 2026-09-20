using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.PayInvoice
{
    public sealed class PayInvoiceCommandHandler(IAppDbContext context) : IRequestHandler<PayInvoiceCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
            if(invoice is null)
                return Result.Failure(Error.NotFound());

            var invoiceResult = invoice.Pay(DateTimeOffset.UtcNow);
            if(invoiceResult.IsError)
                return  Result.Failure(invoiceResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}