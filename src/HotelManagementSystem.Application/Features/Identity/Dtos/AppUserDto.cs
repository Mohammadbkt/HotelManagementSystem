using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementSystem.Application.Features.Identity.Dtos
{
    public sealed record AppUserDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email, 
        string Username,
        string PhoneNumber,
        IReadOnlyCollection<string> Roles,
        IReadOnlyCollection<string> Claims
    );
}