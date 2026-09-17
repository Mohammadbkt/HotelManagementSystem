using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.ApplyInvoiceDiscount
{
    public sealed record ApplyInvoiceDiscountCommand(
        Guid InvoiceId,
        decimal DiscountAmount,
        string DiscountCurrency
    ) : IRequest<Result>;
}