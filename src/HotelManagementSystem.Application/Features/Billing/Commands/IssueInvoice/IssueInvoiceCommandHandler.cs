using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.IssueInvoice
{
    public sealed class IssueInvoiceCommandHandler(IAppDbContext context) : IRequestHandler<IssueInvoiceCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(IssueInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
            if(invoice is null)
                return Result.Failure(Error.NotFound());

            var invoiceResult = invoice.Issue(DateTimeOffset.UtcNow);
            if(invoiceResult.IsError)
                return  Result.Failure(invoiceResult.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}