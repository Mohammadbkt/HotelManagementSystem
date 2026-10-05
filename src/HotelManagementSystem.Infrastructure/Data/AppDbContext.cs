using System.Linq.Expressions;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.Common;
using HotelManagementSystem.Domain.Entities.Billing;
using HotelManagementSystem.Domain.Entities.Billing.Payments;
using HotelManagementSystem.Domain.Entities.Employee;
using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.Entities.Hotels;
using HotelManagementSystem.Domain.Entities.Identity;
using HotelManagementSystem.Domain.Entities.Reservations;
using HotelManagementSystem.Domain.Entities.Rooms;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using HotelManagementSystem.Domain.Entities.Stays;
using HotelManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>, IAppDbContext
    {
        public DbSet<Hotel> Hotels => Set<Hotel>();

        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<Room> Rooms => Set<Room>();

        public DbSet<Reservation> Reservations => Set<Reservation>();

        public DbSet<RoomCategory> RoomCategories => Set<RoomCategory>();

        public DbSet<Guest> Guests => Set<Guest>();

        public DbSet<Stay> Stays => Set<Stay>();

        public DbSet<Invoice> Invoices => Set<Invoice>();

        public DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();

        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Ignore<DomainEvent>();

            builder.Entity<AppUser>().ToTable("Users");
            builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");


            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            
            foreach (var entityType in builder.Model.GetEntityTypes()
                         .Where(t => t.BaseType is null && typeof(ISoftDeletable).IsAssignableFrom(t.ClrType))
                         .ToList())
            {
                var param = Expression.Parameter(entityType.ClrType, "e");
                var isDeleted = Expression.Property(param, nameof(ISoftDeletable.IsDeleted));
                var filter = Expression.Lambda(Expression.Not(isDeleted), param);

                builder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }

    }
}