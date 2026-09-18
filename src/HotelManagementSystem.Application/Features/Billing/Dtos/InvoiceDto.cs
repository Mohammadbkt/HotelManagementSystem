using HotelManagementSystem.Domain.Entities.Billing;

namespace HotelManagementSystem.Application.Features.Billing.Dtos
{
    public sealed record InvoiceDto(
        Guid Id,
        Guid StayId,
        Guid GuestId,
        string GuestName,
        DateTimeOffset? IssuedAt,
        DateTimeOffset? PaidAt,
        InvoiceStatus Status,
        decimal SubTotal,
        decimal DiscountAmount,
        decimal TaxAmount,
        decimal TotalAmount,
        string Currency,
        PaymentDto? Payment,
        IReadOnlyCollection<InvoiceLineItemDto> LineItems

    );
}