using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Application.Features.Billing.Dtos;
using HotelManagementSystem.Application.Features.Billing.Mappers;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Billing.Queries.GetInvoiceById
{
    public sealed class GetInvoiceByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
    {
        private readonly IAppDbContext _context = context;

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
                return Result<InvoiceDto>.Failure(Error.NotFound());

            return Result<InvoiceDto>.Success(invoiceDto);

        }
    }
}