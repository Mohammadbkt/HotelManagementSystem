using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Billing.Dtos;
using HotelManagementSystem.Application.Features.Billing.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Billing.Queries.GetInvoiceById
{
    public sealed class GetInvoiceByIdQueryHandler(IAppDbContext context, ILogger<GetInvoiceByIdQueryHandler> logger) : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetInvoiceByIdQueryHandler> _logger = logger;

        public async Task<Result<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {

            var invoiceDto = await _context.Invoices
                                .AsNoTracking()
                                .Include(i => i.Payment)
                                .Include(i => i.LineItems)
                                .Where(i => i.Id == request.InvoiceId)
                                .ToInvoiceDto()
                                .SingleOrDefaultAsync(cancellationToken);

            if(invoiceDto is null)
            {
                _logger.LogWarning("Invoice {InvoiceId} not found", request.InvoiceId);
                return Result<InvoiceDto>.Failure(ApplicationErrors.Invoice.NotFound);
            }

            _logger.LogInformation("Invoice {InvoiceId} retrieved", request.InvoiceId);
            return Result<InvoiceDto>.Success(invoiceDto);

        }
    }
}