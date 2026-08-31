using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementSystem.Domain.Common
{
    public abstract class AuditableEntity : Entity
    {
        public string? CreatedBy { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public DateTimeOffset LastModifiedUtc { get; private set; }

        public string? LastModifiedBy { get; private set; }


        protected AuditableEntity()
        {


        }

        protected AuditableEntity(Guid Id) : base(Id)
        {
            
        }
    }
}