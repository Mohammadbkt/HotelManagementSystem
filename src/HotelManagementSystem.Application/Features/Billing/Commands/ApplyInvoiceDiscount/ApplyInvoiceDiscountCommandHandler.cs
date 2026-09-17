using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.ApplyInvoiceDiscount
{
    public sealed class ApplyInvoiceDiscountCommandHandler(IAppDbContext context) : IRequestHandler<ApplyInvoiceDiscountCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(ApplyInvoiceDiscountCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
            if (invoice is null)
                return Result.Failure(Error.NotFound());

            var discountResult = Money.Create(request.DiscountAmount, request.DiscountCurrency);
            if (discountResult.IsError)
                return Result.Failure(discountResult.Errors);


            var result = invoice.ApplyDiscount(discountResult.Value); 
            if (result.IsError)
                return Result.Failure(result.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}