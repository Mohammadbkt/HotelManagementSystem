using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.IssueInvoice
{
    public sealed record IssueInvoiceCommand(
        Guid InvoiceId
    ) : IRequest<Result>;
}