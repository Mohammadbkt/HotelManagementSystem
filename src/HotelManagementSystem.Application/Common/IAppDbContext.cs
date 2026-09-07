using HotelManagementSystem.Domain.Entities.Billing;
using HotelManagementSystem.Domain.Entities.Billing.Payments;
using HotelManagementSystem.Domain.Entities.Employee;
using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.Entities.Hotels;
using HotelManagementSystem.Domain.Entities.Reservations;
using HotelManagementSystem.Domain.Entities.Rooms;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using HotelManagementSystem.Domain.Entities.Stay;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Application.Common
{
    public interface IAppDbContext 
    {
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<RoomCategory> RoomTypes { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Stay> Stays { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        
    }
}