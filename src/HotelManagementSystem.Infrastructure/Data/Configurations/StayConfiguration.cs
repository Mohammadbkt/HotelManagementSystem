using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.Entities.Hotels;
using HotelManagementSystem.Domain.Entities.Reservations;
using HotelManagementSystem.Domain.Entities.Rooms;
using HotelManagementSystem.Domain.Entities.Stays;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Data.Configurations;

public sealed class StayConfiguration : IEntityTypeConfiguration<Stay>
{
       public void Configure(EntityTypeBuilder<Stay> builder)
       {
              builder.ToTable("Stays");

              builder.HasKey(s => s.Id);

              builder.HasOne<Guest>()
                             .WithMany()
                             .HasForeignKey(s => s.GuestId)
                             .OnDelete(DeleteBehavior.Restrict);

              builder.HasOne<Room>()
                     .WithMany()
                     .HasForeignKey(s => s.RoomId)
                     .OnDelete(DeleteBehavior.Restrict);

              builder.HasOne<Reservation>()
                     .WithMany()
                     .HasForeignKey(s => s.ReservationId)
                     .OnDelete(DeleteBehavior.SetNull)
                     .IsRequired(false);


              builder.HasOne<Hotel>()
                     .WithMany()
                     .HasForeignKey(s => s.HotelId)
                     .OnDelete(DeleteBehavior.Restrict);


              builder.Property(s => s.GuestId)
                     .IsRequired();

              builder.Property(s => s.RoomId)
                     .IsRequired();

              builder.Property(s => s.ReservationId)
                     .IsRequired(false);

              builder.Property(s => s.CheckInDate)
                     .IsRequired();

              builder.Property(s => s.CheckOutDate)
                     .IsRequired(false);


              builder.HasIndex(s => s.GuestId);
              builder.HasIndex(s => s.RoomId);
              builder.HasIndex(s => s.ReservationId);
              builder.HasIndex(s => s.CheckInDate);

       }
}
