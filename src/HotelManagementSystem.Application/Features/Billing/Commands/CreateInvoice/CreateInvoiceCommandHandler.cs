using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.Entities.Billing;
using HotelManagementSystem.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Features.Billing.Commands.CreateInvoice
{
    public sealed class CreateInvoiceCommandHandler(IAppDbContext context) : IRequestHandler<CreateInvoiceCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<Guid>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var stay = await _context.Stays.FindAsync([request.StayId], cancellationToken);
            if (stay is null)
                return Result<Guid>.Failure(Error.NotFound());

            if (stay.GuestId != request.GuestId)
                return Result<Guid>.Failure(Error.Validation("Invoice.GuestMismatch", "Guest does not match the guest on the stay."));


            var alreadyExists = await _context.Invoices
            .AnyAsync(i => i.StayId == request.StayId && i.Status != InvoiceStatus.Cancelled, cancellationToken);

            if (alreadyExists)
                return Result<Guid>.Failure(Error.Validation("Invoice.AlreadyExistsForStay", "An active invoice already exists for this stay."));

            var room = await _context.Rooms.FindAsync([stay.RoomId], cancellationToken);
            if(room is null)
                return Result<Guid>.Failure(Error.NotFound());


            var roomCategory = await _context.RoomCategories.FindAsync([room.RoomCategoryId], cancellationToken);
            if(roomCategory is null)
                return Result<Guid>.Failure(Error.NotFound());


            var invoiceResult = InvoiceGenerationService.BuildFromStay(stay, roomCategory);
            if (invoiceResult.IsError)
                return Result<Guid>.Failure(invoiceResult.Errors);

            await _context.Invoices.AddAsync(invoiceResult.Value, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);


            return Result<Guid>.Success(invoiceResult.Value.Id);
        }
    }
}