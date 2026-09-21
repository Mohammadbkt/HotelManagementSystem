using HotelManagementSystem.Application.Features.Billing.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Queries.GetInvoiceById
{
    public sealed record GetInvoiceByIdQuery(
        Guid InvoiceId
    ) : IRequest<Result<InvoiceDto>>;
}