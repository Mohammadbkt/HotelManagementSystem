using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Common.Models;
using HotelManagementSystem.Application.Features.Billing.Dtos;
using HotelManagementSystem.Application.Features.Billing.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Billing.Queries.GetInvoices
{
    public sealed class GetInvoicesQueryHandler(IAppDbContext context, ILogger<GetInvoicesQueryHandler> logger) : IRequestHandler<GetInvoicesQuery, Result<PaginatedList<InvoiceDto>>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetInvoicesQueryHandler> _logger = logger;

        public async Task<Result<PaginatedList<InvoiceDto>>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
        {

            var query = _context.Invoices
                        .AsNoTracking()
                        .AsQueryable();

            if(request.StayId.HasValue)
                query = query.Where(i => i.StayId == request.StayId);

            if(request.IssuedFrom.HasValue)
                query = query.Where(i => i.IssuedAt >= request.IssuedFrom);

            if(request.IssuedTo.HasValue)
                query = query.Where(i => i.IssuedAt <= request.IssuedTo);

            if(request.PaidFrom.HasValue)
                query = query.Where(i => i.PaidAt >= request.PaidFrom);

            if(request.PaidTo.HasValue)
                query = query.Where(i => i.PaidAt <= request.PaidTo);

            if(request.AmountFrom.HasValue)
                query = query.Where(i => i.TotalAmount.Amount >= request.AmountFrom);

            if(request.AmountTo.HasValue)
                query = query.Where(i => i.TotalAmount.Amount <= request.AmountTo);

            query = request.SortBy?.ToLowerInvariant() switch
            {
                "amount" => request.SortDirection == SortDirection.Ascending 
                            ? query.OrderBy(i => i.TotalAmount.Amount)
                            : query.OrderByDescending(i => i.TotalAmount.Amount),
                
                "paiddate" => request.SortDirection == SortDirection.Ascending 
                            ? query.OrderBy(i => i.PaidAt)
                            : query.OrderByDescending(i => i.PaidAt),

                _ => query = query.OrderBy(i => i.IssuedAt)
                
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToInvoiceDto()
                        .ToListAsync(cancellationToken);

            var paginatedList = new PaginatedList<InvoiceDto>
            {
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double) request.PageSize),
                Items = items
            };

            _logger.LogInformation("Fetched {TotalCount} invoices (page {PageNumber}/{TotalPages})", totalCount, request.PageNumber, paginatedList.TotalPages);
            return Result<PaginatedList<InvoiceDto>>.Success(paginatedList);
        }
    }
}