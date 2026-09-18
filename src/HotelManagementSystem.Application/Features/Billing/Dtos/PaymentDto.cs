using HotelManagementSystem.Domain.Entities.Billing.Payments;

namespace HotelManagementSystem.Application.Features.Billing.Dtos
{
    public sealed record PaymentDto(
    Guid Id,
    Guid InvoiceId,
    Guid GuestId,
    decimal Amount,
    string Currency,
    PaymentMethods Method,
    DateTimeOffset PaymentDate,
    string? Notes
);
}