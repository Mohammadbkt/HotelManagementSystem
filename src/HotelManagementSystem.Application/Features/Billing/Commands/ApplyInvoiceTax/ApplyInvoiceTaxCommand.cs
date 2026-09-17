using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.ApplyInvoiceTax
{
    public sealed record ApplyInvoiceTaxCommand(
        Guid InvoiceId,
        decimal TaxAmount,
        string TaxCurrency
            ) : IRequest<Result>;
}