using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Billing.Payments;
using HotelManagementSystem.Domain.Entities.Stays;
using HotelManagementSystem.Domain.ValueObjects.Money;

namespace HotelManagementSystem.Domain.Entities.Billing;

public sealed class Invoice : AuditableEntity
{
    private readonly List<InvoiceLineItem> _lineItems = [];

    public IReadOnlyCollection<InvoiceLineItem> LineItems
        => _lineItems.AsReadOnly();

    public Guid StayId { get; private set; }
    public string? Number { get; private set; }

    public DateTimeOffset? IssuedAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }

    public InvoiceStatus Status { get; private set; }

    public Money SubTotal { get; private set; }
    public Money DiscountAmount { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money TotalAmount { get; private set; }

    public Payment? Payment { get; private set; }
    public Stay? Stay { get; private set; }

    // EF Core
    private Invoice()
    {
    }

    private Invoice(
        Guid id,
        Guid stayId,
        string currencyCode)
        : base(id)
    {
        StayId = stayId;
        Status = InvoiceStatus.Draft;

        SubTotal = Money.Zero(currencyCode);
        DiscountAmount = Money.Zero(currencyCode);
        TaxAmount = Money.Zero(currencyCode);
        TotalAmount = Money.Zero(currencyCode);
    }

    public static Result<Invoice> Create(Guid stayId, string currencyCode = "JOD")
    {
        if (stayId == Guid.Empty)
            return Result<Invoice>.Failure(InvoiceErrors.InvalidStayId);

        if (string.IsNullOrWhiteSpace(currencyCode))
            return Result<Invoice>.Failure(MoneyErrors.CurrencyRequired);

        return Result<Invoice>.Success(new Invoice(
            Guid.NewGuid(),
            stayId,
            currencyCode));
    }

    public Result AddLineItem(
        string description,
        int quantity,
        Money unitPrice)
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(InvoiceErrors.CannotModifyInvoice);

        if (unitPrice.Currency != SubTotal.Currency)
            return Result.Failure(MoneyErrors.CurrencyMismatch);

        var lineNumber = _lineItems.Count + 1;

        var result = InvoiceLineItem.Create(description, quantity, lineNumber, unitPrice);

        if (result.IsError)
            return Result.Failure(result.Errors);

        _lineItems.Add(result.Value);

        var recalc = RecalculateTotals();
        if (recalc.IsError)
        {
            _lineItems.Remove(result.Value);
            return recalc;
        }

        return Result.Success();
    }

    public Result Issue(string invoiceNumber, DateTimeOffset issuedAt)
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(InvoiceErrors.CannotIssue);

        if (string.IsNullOrWhiteSpace(invoiceNumber))
            return Result.Failure(InvoiceErrors.InvalidInvoiceNumber);

        if (issuedAt == default)
            return Result.Failure(InvoiceErrors.InvalidIssuedAt);

        if (_lineItems.Count == 0)
            return Result.Failure(InvoiceErrors.EmptyInvoice);


        Number = invoiceNumber.Trim();
        IssuedAt = issuedAt;
        Status = InvoiceStatus.Issued;

        AddDomainEvent(new InvoiceIssuedEvent(Id, issuedAt));


        return Result.Success();
    }

    public Result Pay(DateTimeOffset paidAt)
    {
        if (Status == InvoiceStatus.Paid)
            return Result.Failure(InvoiceErrors.AlreadyPaid);

        if (Status == InvoiceStatus.Cancelled)
            return Result.Failure(InvoiceErrors.CannotPayCancelled);

        if (Status != InvoiceStatus.Issued)
            return Result.Failure(InvoiceErrors.CannotPayInvoice);

        if (paidAt == default)
            return Result.Failure(InvoiceErrors.InvalidPaidAt);

        if (IssuedAt.HasValue && paidAt < IssuedAt.Value)
            return Result.Failure(InvoiceErrors.InvalidPaidAt);

        var paymentResult = Payment.CreateCashPayment(
            Id,
            TotalAmount,
            paidAt);

        if (paymentResult.IsError)
            return Result.Failure(paymentResult.Errors);

        Payment = paymentResult.Value;
        PaidAt = paidAt;
        Status = InvoiceStatus.Paid;

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == InvoiceStatus.Paid)
            return Result.Failure(InvoiceErrors.CannotCancelPaidInvoice);

        if (Status == InvoiceStatus.Cancelled)
            return Result.Failure(InvoiceErrors.AlreadyCancelled);

        Status = InvoiceStatus.Cancelled;

        return Result.Success();
    }

    private Result RecalculateTotals()
    {
        var subtotal = Money.Zero(SubTotal.Currency);
        foreach (var item in _lineItems)
        {
            var sum = subtotal.Add(item.LineTotal);

            if (sum.IsError)
                return Result.Failure(sum.Errors);

            subtotal = sum.Value;
        }

        var afterDiscount = subtotal.Subtract(DiscountAmount);

        if (afterDiscount.IsError)
            return Result.Failure(afterDiscount.Errors);

        var total = afterDiscount.Value.Add(TaxAmount);

        if (total.IsError)
            return Result.Failure(total.Errors);

        SubTotal = subtotal;
        TotalAmount = total.Value;

        return Result.Success();
    }
    public Result ApplyDiscount(Money discount)
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(InvoiceErrors.CannotModifyInvoice);

        if (_lineItems.Count == 0)
            return Result.Failure(InvoiceErrors.CannotModifyEmptyInvoice);


        if (discount.Currency != SubTotal.Currency)
            return Result.Failure(MoneyErrors.CurrencyMismatch);

        if (SubTotal.Subtract(discount).IsError)
            return Result.Failure(InvoiceErrors.DiscountExceedsSubtotal);

        var previous = DiscountAmount;
        DiscountAmount = discount;
        var recalc = RecalculateTotals();
        if (recalc.IsError)
        {
            DiscountAmount = previous;
            return recalc;
        }

        return Result.Success();
    }

    public Result ApplyTax(Money tax)
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(InvoiceErrors.CannotModifyInvoice);

        if (_lineItems.Count == 0)
            return Result.Failure(InvoiceErrors.CannotModifyEmptyInvoice);


        if (tax.Currency != SubTotal.Currency)
            return Result.Failure(MoneyErrors.CurrencyMismatch);

        var previous = TaxAmount;
        TaxAmount = tax;

        var recalc = RecalculateTotals();
        if (recalc.IsError)
        {
            TaxAmount = previous;
            return recalc;
        }

        return Result.Success();
    }
}