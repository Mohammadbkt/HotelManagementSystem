using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementSystem.Application.Features.Guests.Dtos
{
    public sealed record GuestDto(
        Guid Id,
        string FirstName,
        string LastName,
        string? Email,
        string? PhoneNumber,
        DateOnly DateOfBirth
    );
    
}