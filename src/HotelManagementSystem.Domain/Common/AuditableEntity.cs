namespace HotelManagementSystem.Domain.Common
{
    public abstract class AuditableEntity : Entity
    {
        public Guid? CreatedBy { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public DateTimeOffset? LastModifiedUtc { get; private set; }

        public Guid? LastModifiedBy { get; private set; }


        protected AuditableEntity()
        {


        }

        protected AuditableEntity(Guid Id) : base(Id)
        {
            
        }

        public void SetAuditInformation(Guid? userId, DateTimeOffset utcNow)
        {
            if(CreatedAtUtc == default)
            {
                CreatedBy = userId;
                CreatedAtUtc = utcNow;
            }

            LastModifiedBy = userId;
            LastModifiedUtc = utcNow;
        }
    }
}