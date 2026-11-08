using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Billing;
using HotelManagementSystem.Domain.Entities.Reservations;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using HotelManagementSystem.Domain.Entities.Stays;

namespace HotelManagementSystem.Domain.Services;

public static class InvoiceGenerationService
{
    public static Result<Invoice> BuildFromStay(Stay stay, RoomCategory category, Reservation? reservation)
    {
        if (!stay.CheckOutDate.HasValue)
            return Result<Invoice>.Failure(StayErrors.NotCheckedOutYet);

        var checkIn = stay.CheckInDate.Date;
        var checkOut = stay.CheckOutDate.Value.Date;

        if (checkOut <= checkIn)
            return Result<Invoice>.Failure(StayErrors.InvalidCheckOutTime);

        var nights = (checkOut - checkIn).Days;

        var nightlyRate = reservation is not null
                        ? reservation.BasePricePerNight
                        : category.BasePricePerNight;

        var invoiceResult = Invoice.Create(
            stay.Id,
            nightlyRate.Currency);

        if (invoiceResult.IsError)
            return Result<Invoice>.Failure(invoiceResult.Errors);

        var invoice = invoiceResult.Value;

        var lineItemResult = invoice.AddLineItem(
            $"Room charge ({category.Name})",
            nights,
            nightlyRate);

        if (lineItemResult.IsError)
            return Result<Invoice>.Failure(lineItemResult.Errors);

        return Result<Invoice>.Success(invoice);
    }
}