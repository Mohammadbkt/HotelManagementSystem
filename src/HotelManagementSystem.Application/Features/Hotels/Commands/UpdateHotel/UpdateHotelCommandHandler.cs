using HotelManagementSystem.Application.Common.Errors;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Address;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using MediatR;

namespace HotelManagementSystem.Application.Features.Hotels.Commands.UpdateHotel
{
    public sealed class UpdateHotelCommandHandler(IAppDbContext context) : IRequestHandler<UpdateHotelCommand, Result>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
        {
            var hotel = await _context.Hotels.FindAsync([request.HotelId], cancellationToken);
            if (hotel is null)
                return Result.Failure(ApplicationErrors.Hotel.NotFound);

            var addressResult = Address.Create(
            request.AddressLine1, request.City, request.Country,
            request.AddressLine2, request.StateOrProvince, request.PostalCode);

            if(addressResult.IsError)
                return Result.Failure(addressResult.Errors);
                
            var phoneResult = PhoneNumber.Create(request.PhoneNumber);

            if(phoneResult.IsError)
            {
                return Result.Failure(phoneResult.Errors);
            }

            var result =  hotel.UpdateInfo(request.Name, addressResult.Value, phoneResult.Value);
            if(result.IsError)
                return Result.Failure(result.Errors);
            
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}