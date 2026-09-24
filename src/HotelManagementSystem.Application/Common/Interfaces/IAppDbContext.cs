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
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        public DbSet<Hotel> Hotels { get; }
        public DbSet<Employee> Employees { get; }
        public DbSet<Room> Rooms { get; }
        public DbSet<Reservation> Reservations { get; }
        public DbSet<RoomCategory> RoomCategories { get; }
        public DbSet<Guest> Guests { get; }
        public DbSet<Stay> Stays { get; }
        public DbSet<Invoice> Invoices { get; }
        public DbSet<InvoiceLineItem> InvoiceLineItems { get; }
        public DbSet<Payment> Payments { get; }
        public DbSet<RefreshToken> RefreshTokens { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    }
}