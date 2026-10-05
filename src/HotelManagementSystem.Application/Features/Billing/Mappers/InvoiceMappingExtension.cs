using System.Linq.Expressions;
using HotelManagementSystem.Application.Features.Billing.Dtos;
using HotelManagementSystem.Domain.Entities.Billing;

namespace HotelManagementSystem.Application.Features.Billing.Mappers
{
    public static class InvoiceMappingExtension
    {
        public static readonly Expression<Func<Invoice, InvoiceDto>> ProjectionExpression =
            i => new InvoiceDto(
                i.Id,
                i.StayId,
                i.GuestId,
                i.IssuedAt,
                i.PaidAt,
                i.Status,
                i.SubTotal.Amount,
                i.DiscountAmount.Amount,
                i.TaxAmount.Amount,
                i.TotalAmount.Amount,
                i.SubTotal.Currency,
                i.Payment == null
                    ? null
                    : new PaymentDto(
                        i.Payment.Id,
                        i.Payment.InvoiceId,
                        i.Payment.GuestId,
                        i.Payment.Amount.Amount,
                        i.Payment.Amount.Currency,
                        i.Payment.Method,
                        i.Payment.PaymentDate,
                        i.Payment.Notes
                    ),
                i.LineItems
                    .Select(li => new InvoiceLineItemDto(
                        li.Id,
                        li.LineNumber,
                        li.Description,
                        li.Quantity,
                        li.UnitPrice.Amount,
                        li.LineTotal.Amount,
                        li.UnitPrice.Currency
                    ))
                    .ToList()
            );

        
        private static readonly Func<Invoice, InvoiceDto>  _compiledProjection 
            = ProjectionExpression.Compile();

        
        public static InvoiceDto ToDto(this Invoice invoice) => _compiledProjection(invoice);

        public static IQueryable<InvoiceDto> ToInvoiceDto(this IQueryable<Invoice> query) 
            => query.Select(ProjectionExpression);

    }
}