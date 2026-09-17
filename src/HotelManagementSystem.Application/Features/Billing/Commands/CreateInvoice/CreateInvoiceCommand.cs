using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Commands.CreateInvoice
{
    public sealed record CreateInvoiceCommand(
        Guid StayId,
        Guid GuestId,
        string CurrencyCode = "JOD"
    ) : IRequest<Result<Guid>>;

}