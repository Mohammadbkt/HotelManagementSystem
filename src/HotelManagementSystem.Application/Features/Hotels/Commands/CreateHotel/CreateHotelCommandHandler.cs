using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Address;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using HotelManagementSystem.Domain.Entities.Hotels;
using MediatR;
using HotelManagementSystem.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Features.Hotels.Commands.CreateHotel
{
    public sealed class CreateHotelCommandHandler(IAppDbContext context, ILogger<CreateHotelCommandHandler> logger) : IRequestHandler<CreateHotelCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<CreateHotelCommandHandler> _logger = logger;


        public async Task<Result<Guid>> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
        {

            var addressResult = Address.Create(
            request.AddressLine1, request.City, request.Country,
            request.AddressLine2, request.StateOrProvince, request.PostalCode);
            
            if(addressResult.IsError)
            {
                _logger.LogWarning("Invalid address when creating hotel: {Errors}", addressResult.Errors);
                return Result<Guid>.Failure(addressResult.Errors);
            }

            var phoneResult = PhoneNumber.Create(request.PhoneNumber);

            if(phoneResult.IsError)
            {
                _logger.LogWarning("Invalid phone when creating hotel: {Errors}", phoneResult.Errors);
                return Result<Guid>.Failure(phoneResult.Errors);
            }

            var hotelResult = Hotel.Create(request.Name, addressResult.Value, phoneResult.Value);
            if(hotelResult.IsError)
            {
                _logger.LogWarning("Failed to create hotel: {Errors}", hotelResult.Errors);
                return Result<Guid>.Failure(hotelResult.Errors);
            }

            await _context.Hotels.AddAsync(hotelResult.Value);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Hotel {HotelId} created", hotelResult.Value.Id);
            return Result<Guid>.Success(hotelResult.Value.Id); 
        }
    }
}