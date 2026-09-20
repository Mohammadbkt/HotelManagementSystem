using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Address;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using HotelManagementSystem.Domain.Entities.Hotels;
using MediatR;
using HotelManagementSystem.Application.Common.Interfaces;

namespace HotelManagementSystem.Application.Features.Hotels.Commands.CreateHotel
{
    public sealed class CreateHotelCommandHandler(IAppDbContext context) : IRequestHandler<CreateHotelCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;


        public async Task<Result<Guid>> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
        {
            var addressResult = Address.Create(
            request.AddressLine1, request.City, request.Country,
            request.AddressLine2, request.StateOrProvince, request.PostalCode);
            
            if(addressResult.IsError)
            {
                return Result<Guid>.Failure(addressResult.Errors);
            }

            var phoneResult = PhoneNumber.Create(request.PhoneNumber);

            if(phoneResult.IsError)
            {
                return Result<Guid>.Failure(phoneResult.Errors);
            }

            var hotelResult = Hotel.Create(request.Name, addressResult.Value, phoneResult.Value);
            if(hotelResult.IsError)
            {
                return Result<Guid>.Failure(hotelResult.Errors);
            }

            await _context.Hotels.AddAsync(hotelResult.Value);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(hotelResult.Value.Id); 
        }
    }
}