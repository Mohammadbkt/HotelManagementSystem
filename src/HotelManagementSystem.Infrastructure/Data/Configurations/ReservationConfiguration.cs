using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.Entities.Hotels;
using HotelManagementSystem.Domain.Entities.Reservations;
using HotelManagementSystem.Domain.Entities.Rooms;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using HotelManagementSystem.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Data.Configurations
{
       public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
       {
              public void Configure(EntityTypeBuilder<Reservation> builder)
              {
                     builder.ToTable("Reservations");

                     builder.HasKey(r => r.Id);

                     builder.HasOne<Guest>()
                             .WithMany()
                             .HasForeignKey(r => r.GuestId)
                             .OnDelete(DeleteBehavior.Restrict);


                     builder.HasOne<Room>()
                         .WithMany()
                         .HasForeignKey(r => r.RoomId)
                         .OnDelete(DeleteBehavior.Restrict);


                     builder.HasOne<Hotel>()
                            .WithMany()
                            .HasForeignKey(s => s.HotelId)
                            .OnDelete(DeleteBehavior.Restrict);


                     builder.HasOne<RoomCategory>()
                            .WithMany()
                            .HasForeignKey(s => s.RoomCategoryId)
                            .OnDelete(DeleteBehavior.Restrict);

                     builder.Property(r => r.GuestId)
                            .IsRequired();

                     builder.Property(r => r.RoomId)
                            .IsRequired();

                     builder.Property(r => r.Status)
                            .IsRequired()
                            .HasConversion<string>()
                            .HasMaxLength(20);

                     builder.Property(r => r.CheckInDate)
                            .IsRequired();

                     builder.Property(r => r.CheckOutDate)
                            .IsRequired();

                     builder.ComplexProperty(p => p.BasePricePerNight,
                            m => m.ConfigureMoney("Amount", "Currency"));

                     builder.ComplexProperty(p => p.TotalAmount,
                            m => m.ConfigureMoney("Amount", "Currency"));


                     builder.HasIndex(r => r.GuestId);
                     builder.HasIndex(r => r.RoomId);
                     builder.HasIndex(r => r.Status);
              }
       }
}