using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.AddInvoiceLineItem
{
    public sealed record AddInvoiceLineItemCommand(
        Guid InvoiceId,
        string Description,
        int Quantity,
        decimal UnitPriceAmount,
        string UnitPriceCurrency
    ) : IRequest<Result>;
}