using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Billing;
using HotelManagementSystem.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Billing.Commands.CreateInvoice
{
    public sealed class CreateInvoiceCommandHandler(IAppDbContext context, ILogger<CreateInvoiceCommandHandler> logger) : IRequestHandler<CreateInvoiceCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<CreateInvoiceCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var stay = await _context.Stays.FindAsync([request.StayId], cancellationToken);
            if (stay is null)
            {
                _logger.LogWarning(
               "Cannot create invoice: stay {StayId} was not found",
               request.StayId);

                return Result<Guid>.Failure(ApplicationErrors.Stay.NotFound);
            }
            if (stay.GuestId != request.GuestId)
            {
                _logger.LogWarning(
                "Cannot create invoice for stay {StayId}: requested guest {RequestedGuestId} does not match stay guest",
                request.StayId,
                request.GuestId);

                return Result<Guid>.Failure(ApplicationErrors.Invoice.GuestMismatch);
            }

            var alreadyExists = await _context.Invoices
            .AnyAsync(i => i.StayId == request.StayId && i.Status != InvoiceStatus.Cancelled, cancellationToken);

            if (alreadyExists)
            {
                _logger.LogWarning(
                "Cannot create invoice: an active invoice already exists for stay {StayId}",
                request.StayId);

                return Result<Guid>.Failure(ApplicationErrors.Invoice.AlreadyExistsForStay);
            }

            var room = await _context.Rooms.FindAsync([stay.RoomId], cancellationToken);
            if (room is null)
            {
                _logger.LogWarning(
                "Cannot create invoice for stay {StayId}: room {RoomId} was not found",
                request.StayId,
                stay.RoomId);

                return Result<Guid>.Failure(ApplicationErrors.Room.NotFound);
            }

            var roomCategory = await _context.RoomCategories.FindAsync([room.RoomCategoryId], cancellationToken);
            if (roomCategory is null)
            {
                _logger.LogWarning(
                               "Cannot create invoice for stay {StayId}: room category {RoomCategoryId} was not found",
                               request.StayId,
                               room.RoomCategoryId);

                return Result<Guid>.Failure(ApplicationErrors.RoomCategory.NotFound);
            }

            var invoiceResult = InvoiceGenerationService.BuildFromStay(stay, roomCategory);
            if (invoiceResult.IsError)
            {
                _logger.LogWarning(
                                "Invoice generation failed for stay {StayId}: {ErrorCode}",
                                request.StayId,
                                invoiceResult.Errors);

                return Result<Guid>.Failure(invoiceResult.Errors);
            }
            await _context.Invoices.AddAsync(invoiceResult.Value, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

           _logger.LogInformation(
          "Invoice {InvoiceId} created for stay {StayId}, guest {GuestId}, and room {RoomId}",
          invoiceResult.Value.Id,
          request.StayId,
          stay.GuestId,
          stay.RoomId);


            return Result<Guid>.Success(invoiceResult.Value.Id);
        }
    }
}