using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Address;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Hotels.Commands.UpdateHotel
{
    public sealed class UpdateHotelCommandHandler(IAppDbContext context, ILogger<UpdateHotelCommandHandler> logger) 
    : IRequestHandler<UpdateHotelCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateHotelCommandHandler> _logger = logger;
        public async Task<Result> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
        {
            var hotel = await _context.Hotels.FindAsync([request.HotelId], cancellationToken);
            if (hotel is null)
            {
                _logger.LogWarning("Hotel {HotelId} not found", request.HotelId);
                return Result.Failure(ApplicationErrors.Hotel.NotFound);
            }

            var addressResult = Address.Create(
            request.AddressLine1, request.City, request.Country,
            request.AddressLine2, request.StateOrProvince, request.PostalCode);

            if(addressResult.IsError)
            {
                _logger.LogWarning("Invalid address when updating hotel {HotelId}: {Errors}", request.HotelId, addressResult.Errors);
                return Result.Failure(addressResult.Errors);
            }
                
            var phoneResult = PhoneNumber.Create(request.PhoneNumber);

            if(phoneResult.IsError)
            {
                _logger.LogWarning("Invalid phone when updating hotel {HotelId}: {Errors}", request.HotelId, phoneResult.Errors);
                return Result.Failure(phoneResult.Errors);
            }

            var result =  hotel.UpdateInfo(request.Name, addressResult.Value, phoneResult.Value);
            if(result.IsError)
            {
                _logger.LogWarning("Failed to update hotel {HotelId}: {Errors}", request.HotelId, result.Errors);
                return Result.Failure(result.Errors);
            }
            
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Hotel {HotelId} updated", request.HotelId);
            return Result.Success();
        }
    }
}