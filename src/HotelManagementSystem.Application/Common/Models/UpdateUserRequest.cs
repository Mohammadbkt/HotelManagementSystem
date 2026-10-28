using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementSystem.Application.Common.Models
{
    public sealed record UpdateUserRequest(
    string FirstName,
    string LastName,
    string? PhoneNumber);
}