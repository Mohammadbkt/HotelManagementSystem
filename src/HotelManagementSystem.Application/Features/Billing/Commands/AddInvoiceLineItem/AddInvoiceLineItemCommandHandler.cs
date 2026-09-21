using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.AddInvoiceLineItem
{
    public sealed class AddInvoiceLineItemCommandHandler(IAppDbContext context) : IRequestHandler<AddInvoiceLineItemCommand, Result>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result> Handle(AddInvoiceLineItemCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
            if (invoice is null)
                return Result.Failure(ApplicationErrors.Room.NotFound);

            var amountResult = Money.Create(request.UnitPriceAmount, request.UnitPriceCurrency);
            if (amountResult.IsError)
                return Result.Failure(amountResult.Errors);

            var result = invoice.AddLineItem(request.Description, request.Quantity, amountResult.Value);
            if (result.IsError)
                return Result.Failure(result.Errors);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}