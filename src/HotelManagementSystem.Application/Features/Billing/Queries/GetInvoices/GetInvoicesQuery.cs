using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Billing.Dtos;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Billing;
using MediatR;

namespace HotelManagementSystem.Application.Features.Billing.Queries.GetInvoices
{
    public sealed record GetInvoicesQuery(
        Guid? HotelId = null,
        Guid? GuestId = null,
        Guid? StayId = null,
        // string? SearchTerm = null,
        SortDirection SortDirection = SortDirection.Ascending,
        string? SortBy = null,
        DateTimeOffset? IssuedFrom = null,
        DateTimeOffset? IssuedTo = null,
        DateTimeOffset? PaidFrom = null,
        DateTimeOffset? PaidTo = null,
        InvoiceStatus? Status = null,
        decimal? AmountFrom = null,
        decimal? AmountTo = null,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<Result<PaginatedList<InvoiceDto>>>;
}