using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Billing.Payments
{
    public static class PaymentErrors
    {
        public static Error InvalidInvoiceId =>
             Error.Validation(
                "InvalidInvoiceId",
                "The invoice ID is invalid.");

        public static Error InvalidAmount =>
            Error.Validation(
                "InvalidAmount",
                "The payment amount is invalid.");

        public static Error InvalidPaidAt =>
            Error.Validation(
                "InvalidPaidAt",
                "The payment date and time is invalid.");
        
        public static Error InvalidGuestId =>
            Error.Validation(
                "InvalidGuestId",
                "The guest ID is invalid.");
    }
}