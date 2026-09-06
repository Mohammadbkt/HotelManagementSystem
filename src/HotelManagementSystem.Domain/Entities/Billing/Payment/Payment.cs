using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Money;

namespace HotelManagementSystem.Domain.Entities.Billing.Payments
{
    public sealed class Payment : AuditableEntity
    {
        public Guid InvoiceId { get; private set; }
        public Guid GuestId { get; private set; }
        public Guid ReceivedByUserId { get; private set; }

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
            Guid guestId,
            Guid receivedByUserId,
            Money amount,
            DateTimeOffset paymentDate,
            string? notes)
            : base(id)
        {
            InvoiceId = invoiceId;
            GuestId = guestId;
            ReceivedByUserId = receivedByUserId;
            Amount = amount;
            Method = PaymentMethods.Cash;
            PaymentDate = paymentDate;
            Notes = notes;
        }

        public static Result<Payment> CreateCashPayment(
            Guid invoiceId,
            Guid guestId,
            Guid receivedByUserId,
            Money amount,
            DateTimeOffset paymentDate,
            string? notes = null)
        {
            if (invoiceId == Guid.Empty)
                return PaymentErrors.InvalidInvoiceId;

            if (guestId == Guid.Empty)
                return PaymentErrors.InvalidGuestId;

            if (amount.Amount <= 0)
                return PaymentErrors.InvalidAmount;

            if (paymentDate == default)
                return PaymentErrors.InvalidPaidAt;

            return new Payment(
                Guid.NewGuid(),
                invoiceId,
                guestId,
                receivedByUserId,
                amount,
                paymentDate,
                notes);
        }
    }
}