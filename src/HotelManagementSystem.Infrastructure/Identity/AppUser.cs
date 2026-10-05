using HotelManagementSystem.Domain.ValueObjects.Email;
using Microsoft.AspNetCore.Identity;

namespace HotelManagementSystem.Infrastructure.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        

    };
}