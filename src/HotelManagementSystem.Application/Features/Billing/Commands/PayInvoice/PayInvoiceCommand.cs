using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.PayInvoice
{
    public sealed record PayInvoiceCommand(
        Guid InvoiceId
    ) : IRequest<Result>;
}