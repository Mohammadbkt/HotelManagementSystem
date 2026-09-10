using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementSystem.Domain.Common
{
    public class SoftDeletableEntity : AuditableEntity, ISoftDeletable
    {
        public bool IsDeleted { get; private set; }

        public DateTimeOffset? DeletedAt { get; private set; }

        public Guid? DeletedBy { get; private set; }


        protected SoftDeletableEntity() { }
        protected SoftDeletableEntity(Guid id) : base(id) { }

        public void Delete(Guid deletedBy)
        {

            if (IsDeleted)
                return;

            IsDeleted = true;
            DeletedAt = DateTimeOffset.UtcNow;
            DeletedBy = deletedBy;
        }

        public void Restore()
        {
            if (!IsDeleted)
                return;

            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;

        }
    }
}