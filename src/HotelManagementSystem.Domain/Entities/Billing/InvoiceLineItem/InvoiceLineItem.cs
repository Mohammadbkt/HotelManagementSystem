using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;

namespace HotelManagementSystem.Domain.Entities.Billing;

public sealed class InvoiceLineItem : Entity
{
    public string Description { get; private set; } = null!;
    public int Quantity { get; private set; }
    public int LineNumber { get; private set; }

    public Money UnitPrice { get; private set; }
    public Money LineTotal { get; private set; }

    // EF Core
    private InvoiceLineItem()
    {
    }

    private InvoiceLineItem(
        Guid id,
        string description,
        int quantity,
        int lineNumber,
        Money unitPrice)
        : base(id)
    {
        Description = description;
        Quantity = quantity;
        LineNumber = lineNumber;
        UnitPrice = unitPrice;
        LineTotal = unitPrice.Multiply(quantity).Value;
    }

    public static Result<InvoiceLineItem> Create(
        string description,
        int quantity,
        int lineNumber,
        Money unitPrice)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result<InvoiceLineItem>.Failure(InvoiceLineItemErrors.InvalidDescription);

        if (quantity <= 0)
            return Result<InvoiceLineItem>.Failure(InvoiceLineItemErrors.InvalidQuantity);

        if (lineNumber <= 0)
            return Result<InvoiceLineItem>.Failure(InvoiceLineItemErrors.InvalidLineNumber);

        if (unitPrice.Amount <= 0)
            return Result<InvoiceLineItem>.Failure(InvoiceLineItemErrors.InvalidUnitPrice);

        return Result<InvoiceLineItem>.Success(new InvoiceLineItem(
            Guid.NewGuid(),
            description.Trim(),
            quantity,
            lineNumber,
            unitPrice));
    }
}