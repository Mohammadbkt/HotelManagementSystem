using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Billing.Payments;
using HotelManagementSystem.Domain.ValueObjects.Money;

namespace HotelManagementSystem.Domain.Entities.Billing;

public sealed class Invoice : AuditableEntity
{
    private readonly List<InvoiceLineItem> _lineItems = [];

    public IReadOnlyCollection<InvoiceLineItem> LineItems
        => _lineItems.AsReadOnly();

    public Guid StayId { get; private set; }
    public Guid GuestId { get; private set; }

    public DateTimeOffset? IssuedAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }

    public InvoiceStatus Status { get; private set; }

    public Money SubTotal { get; private set; }
    public Money DiscountAmount { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money TotalAmount { get; private set; }

    public Payment? Payment { get; private set; }

    // EF Core
    private Invoice()
    {
    }

    private Invoice(
        Guid id,
        Guid stayId,
        Guid guestId,
        string currencyCode)
        : base(id)
    {
        StayId = stayId;
        GuestId = guestId;

        Status = InvoiceStatus.Draft;

        SubTotal = Money.Zero(currencyCode);
        DiscountAmount = Money.Zero(currencyCode);
        TaxAmount = Money.Zero(currencyCode);
        TotalAmount = Money.Zero(currencyCode);
    }

    public static Result<Invoice> Create(
        Guid stayId,
        Guid guestId,
        string currencyCode = "JOD")
    {
        if (stayId == Guid.Empty)
            return Result<Invoice>.Failure(InvoiceErrors.InvalidStayId);

        if (guestId == Guid.Empty)
            return Result<Invoice>.Failure(InvoiceErrors.InvalidGuestId);

        if (string.IsNullOrWhiteSpace(currencyCode))
            return Result<Invoice>.Failure(MoneyErrors.CurrencyRequired);

        return Result<Invoice>.Success(new Invoice(
            Guid.NewGuid(),
            stayId,
            guestId,
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

        var result = InvoiceLineItem.Create(
            description,
            quantity,
            lineNumber,
            unitPrice);

        if (result.IsError)
            return Result.Failure(result.Errors);

        _lineItems.Add(result.Value);

        RecalculateTotals();

        return Result.Success();
    }

    public Result Issue(DateTimeOffset issuedAt)
    {
        if (Status != InvoiceStatus.Draft)
            return Result.Failure(InvoiceErrors.CannotIssue);

        if (issuedAt == default)
            return Result.Failure(InvoiceErrors.InvalidIssuedAt);

        if (_lineItems.Count == 0)
            return Result.Failure(InvoiceErrors.EmptyInvoice);

        IssuedAt = issuedAt;
        Status = InvoiceStatus.Issued;

        AddDomainEvent(new InvoiceIssuedEvent(Id, GuestId, issuedAt));


        return Result.Success();
    }

    public Result Pay(Guid receivedByUserId, DateTimeOffset paidAt)
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
            GuestId,
            receivedByUserId,
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

        foreach (var lineItem in _lineItems)
        {
            var result = subtotal.Add(lineItem.LineTotal);

            if (result.IsError)
            {
                return Result.Failure(result.Errors);
            }

            subtotal = result.Value;
        }

        SubTotal = subtotal;

        var afterDiscount = SubTotal.Subtract(DiscountAmount);

        if (afterDiscount.IsError)
        {
            return Result.Failure(afterDiscount.Errors);
        }

        var total = afterDiscount.Value.Add(TaxAmount);

        if (total.IsError)
        {
            return Result.Failure(total.Errors);
        }

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

        DiscountAmount = discount;
        RecalculateTotals();

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

        TaxAmount = tax;

        RecalculateTotals();

        return Result.Success();
    }
}