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
        string UserName,
        Guid? HotelId,
        string? PhoneNumber,
        bool IsActive,
        IReadOnlyCollection<string> Roles
    );
}