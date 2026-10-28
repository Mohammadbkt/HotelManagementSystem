using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common;

namespace HotelManagementSystem.Domain.Entities.Billing
{
    public sealed class InvoiceIssuedEvent: DomainEvent
    {
        public Guid InvoiceId { get; }
        public DateTimeOffset IssuedAt { get; }

        public InvoiceIssuedEvent(Guid invoiceId, DateTimeOffset issuedAt)
        {
            InvoiceId = invoiceId;
            IssuedAt = issuedAt;
        }
    }
}