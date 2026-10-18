using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.ValueObjects.Email;
using Microsoft.AspNetCore.Identity;

namespace HotelManagementSystem.Infrastructure.Identity
{
    public class AppUser : IdentityUser<Guid>, ISoftDeletable
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; private set; }
        public Guid? DeletedBy { get; private set; }

        public void Delete(Guid deletedBy)
        {
            if (IsDeleted) return;

            IsDeleted = true;
            DeletedAt = DateTimeOffset.UtcNow;
            DeletedBy = deletedBy;
        }

        public void Restore()
        {
            if (!IsDeleted) return;

            IsDeleted = false; 
            DeletedAt = null;
            DeletedBy = null;
        }
    }
}