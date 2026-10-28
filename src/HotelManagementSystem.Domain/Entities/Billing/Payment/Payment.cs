using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;

namespace HotelManagementSystem.Domain.Entities.Billing.Payments
{
    public sealed class Payment : AuditableEntity
    {
        public Guid InvoiceId { get; private set; }
        //public Guid ReceivedByUserId { get; private set; }

        public Money Amount { get; private set; }
        public PaymentMethods Method { get; private set; }

        public DateTimeOffset PaymentDate { get; private set; }
        public string? Notes { get; private set; }

        private Payment()
        {
        }

        private Payment(
            Guid id,
            Guid invoiceId,
            Money amount,
            DateTimeOffset paymentDate,
            string? notes)
            : base(id)
        {
            InvoiceId = invoiceId;
            Amount = amount;
            Method = PaymentMethods.Cash;
            PaymentDate = paymentDate;
            Notes = notes;
        }

        public static Result<Payment> CreateCashPayment(
            Guid invoiceId,
            Money amount,
            DateTimeOffset paymentDate,
            string? notes = null)
        {
            if (invoiceId == Guid.Empty)
                return Result<Payment>.Failure(PaymentErrors.InvalidInvoiceId);

            if (amount.Amount <= 0)
                return Result<Payment>.Failure(PaymentErrors.InvalidAmount);

            if (paymentDate == default)
                return Result<Payment>.Failure(PaymentErrors.InvalidPaidAt);

            return Result<Payment>.Success(new Payment(
                Guid.NewGuid(),
                invoiceId,
                amount,
                paymentDate,
                notes));
        }
    }
}