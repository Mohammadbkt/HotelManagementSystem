using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;

namespace HotelManagementSystem.Domain.Entities.Billing
{
    public static class InvoiceLineItemErrors
    {
        public static Error InvoiceIdRequired => Error.Validation(
        code: "InvoiceLineItemErrors.InvoiceIdRequired",
        description: "InvoiceId is required.");

        public static Error InvalidLineNumber => Error.Validation(
            code: "InvoiceLineItemErrors.InvalidLineNumber",
            description: "Line number must be greater than 0.");

        public static Error InvalidDescription => Error.Validation(
            code: "InvoiceLineItemErrors.InvalidDescription",
            description: "Description is invalid.");

        public static Error InvalidQuantity => Error.Validation(
            code: "InvoiceLineItemErrors.InvalidQuantity",
            description: "Quantity must be greater than 0.");

        public static Error InvalidUnitPrice => Error.Validation(
            code: "InvoiceLineItemErrors.InvalidUnitPrice",
            description: "Unit price must be greater than 0.");

    }
}