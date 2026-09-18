namespace HotelManagementSystem.Application.Features.Billing.Dtos
{
    public sealed record InvoiceLineItemDto(
        Guid Id,
        int LineNumber,
        string Description,
        int Quantity,
        decimal UnitPrice,
        decimal LineTotal,
        string Currency
);
}