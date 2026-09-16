using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Billing;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using HotelManagementSystem.Domain.Entities.Stays;

namespace HotelManagementSystem.Domain.Services;

public static class InvoiceGenerationService
{
    public static Result<Invoice> BuildFromStay(Stay stay, RoomCategory category)
    {
        if (!stay.CheckOutDate.HasValue)
             return Result<Invoice>.Failure(StayErrors.NotCheckedOutYet);

        var nights = (int)Math.Ceiling(
            (stay.CheckOutDate.Value - stay.CheckInDate).TotalDays);

        if (nights < 1)
            nights = 1;

        var invoiceResult = Invoice.Create(
            stay.Id,
            stay.GuestId,
            category.BasePricePerNight.Currency);

        if (invoiceResult.IsError)
            return Result<Invoice>.Failure(invoiceResult.Errors);

        var invoice = invoiceResult.Value;

        var lineItemResult = invoice.AddLineItem(
            $"Room charge ({category.Name})",
            nights,
            category.BasePricePerNight);

        if (lineItemResult.IsError)
            return Result<Invoice>.Failure(lineItemResult.Errors);

        return Result<Invoice>.Success(invoice);
    }
}