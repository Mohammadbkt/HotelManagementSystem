using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Domain.Common.Result;
using HotelManagementSystem.Domain.ValueObjects.Address;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using HotelManagementSystem.Domain.Entities.Hotels;
using MediatR;
using HotelManagementSystem.Application.Common;

namespace HotelManagementSystem.Application.Features.Hotel.Commands.CreateHotel
{
    public sealed class CreateHotelCommandHandler(IAppDbContext context) : IRequestHandler<CreateHotelCommand, Result<Guid>>
    {
        private readonly IAppDbContext _context = context;


        public async Task<Result<Guid>> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
        {
            var addressResult = Address.Create(request.Street, request.City, request.Country);
            if(addressResult.IsError)
            {
                return Result<Guid>.Failure(addressResult.Errors);
            }

            var phoneResult = PhoneNumber.Create(request.Phone);
            if(phoneResult.IsError)
            {
                return Result<Guid>.Failure(phoneResult.Errors);
            }

            var hotelResult = Hotel.Create(request.Name, addressResult.Value, phoneResult.Value);
            if(hotelResult.IsError)
            {
                return Result<Guid>.Failure(hotelResult.Errors);
            }

            _context.Hotels.Add(hotelResult.Value);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(hotelResult.Value.Id); 
        }
    }
}