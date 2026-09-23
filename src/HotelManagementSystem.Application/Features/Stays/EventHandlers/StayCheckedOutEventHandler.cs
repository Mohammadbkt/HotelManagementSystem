using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Entities.Billing;
using HotelManagementSystem.Domain.Entities.Stays;
using HotelManagementSystem.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Stays.EventHandlers;

public sealed class StayCheckedOutEventHandler(IAppDbContext context, ILogger<StayCheckedOutEventHandler> logger) : INotificationHandler<StayCheckedOutEvent>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger _logger = logger;

    public async Task Handle(StayCheckedOutEvent notification, CancellationToken cancellationToken)
    {
        var hasInvoice = await _context.Invoices.AnyAsync(i =>
                                       i.StayId == notification.StayId
                                    && i.Status != InvoiceStatus.Cancelled,
                                    cancellationToken);

        if (hasInvoice)
            return;

        var stay = await _context.Stays.FindAsync([notification.StayId], cancellationToken);

        var room = await _context.Rooms.FindAsync([notification.RoomId], cancellationToken);

        var category = room is null
            ? null
            : await _context.RoomCategories.FindAsync([room.RoomCategoryId], cancellationToken);

        if (stay is null || room is null || category is null)
        {
            _logger.LogWarning("Draft invoice not created for stay {StayId}: missing stay, room or category.",
                notification.StayId);
            return;
        }

        var invoiceResult = InvoiceGenerationService.BuildFromStay(stay, category);
        if (invoiceResult.IsError)
        {
            _logger.LogWarning("Draft invoice not created for stay {StayId}: {Errors}",
               notification.StayId, invoiceResult.Errors);
            return;
        }

        await _context.Invoices.AddAsync(invoiceResult.Value, cancellationToken);

    }
}
