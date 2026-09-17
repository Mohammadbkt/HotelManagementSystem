using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.CancelInvoice
{
    public sealed record CancelInvoiceCommand(
        Guid InvoiceId
    ) : IRequest<Result>;
}