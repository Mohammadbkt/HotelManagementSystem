using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HotelManagementSystem.Infrastructure.Interceptors
{
    public sealed class AuditableEntityInterceptor(ICurrentUserService user, TimeProvider timeProvider) : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _user = user;
        private readonly TimeProvider _timeProvider = timeProvider;

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? context)
        {
            if (context == null)
                return;

            var utcNow = _timeProvider.GetUtcNow();
            
            foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {

                    entry.Entity.SetAuditInformation(_user.UserId, utcNow);
                }
            }
        }


    }
}
