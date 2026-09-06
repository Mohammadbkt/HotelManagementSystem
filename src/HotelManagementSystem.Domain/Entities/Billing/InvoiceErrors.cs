using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Billing;

public static class InvoiceErrors
{
    public static readonly Error InvalidStayId = Error.Validation(
        "Invoice.StayId.Invalid",
        "StayId is invalid");

    public static readonly Error InvalidGuestId = Error.Validation(
        "Invoice.GuestId.Invalid",
        "GuestId is invalid");

    public static readonly Error InvalidIssuedAt = Error.Validation(
        "Invoice.IssuedAt.Invalid",
        "IssuedAt is invalid");

    public static readonly Error AlreadyPaid = Error.Validation(
        "Invoice.AlreadyPaid",
        "Invoice is already paid");

    public static readonly Error CannotPayCancelled = Error.Validation(
        "Invoice.CannotPayCancelled",
        "Cannot pay a cancelled invoice");

    public static readonly Error CannotModifyInvoice = Error.Validation(
        "Invoice.CannotModifyInvoice",
        "Cannot modify an invoice that is not in draft status");

    public static readonly Error EmptyInvoice = Error.Validation(
        "Invoice.EmptyInvoice",
        "Cannot issue an empty invoice");

    public static readonly Error CannotCancelPaidInvoice = Error.Validation(
        "Invoice.CannotCancelPaidInvoice",
        "Cannot cancel a paid invoice");

    public static readonly Error AlreadyCancelled = Error.Validation(
        "Invoice.AlreadyCancelled",
        "Invoice is already cancelled");

    public static readonly Error CannotPayInvoice = Error.Validation(
        "Invoice.CannotPayInvoice",
        "Cannot pay an invoice that is not in issued status");

    public static readonly Error InvalidPaidAt = Error.Validation(
        "Invoice.PaidAt.Invalid",
        "PaidAt is invalid");

    public static readonly Error CannotIssue = Error.Validation(
       "Invoice.CannotIssue",
       "Cannot issue an invoice that is not in draft status");

    public static readonly Error CannotModifyEmptyInvoice = Error.Validation(
    "Invoice.CannotModifyEmptyInvoice",
    "Cannot apply a discount or tax before adding line items");
    

    public static readonly Error DiscountExceedsSubtotal = Error.Validation(
    "Invoice.DiscountExceedsSubtotal",
    "Discount cannot exceed the invoice subtotal");

    public static readonly Error IncorrectPaymentAmount = Error.Validation(
        "Invoice.IncorrectPaymentAmount",
        "Payment amount must exactly match the invoice total");
}
