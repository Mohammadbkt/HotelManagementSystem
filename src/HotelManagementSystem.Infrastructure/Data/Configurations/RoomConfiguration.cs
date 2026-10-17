using HotelManagementSystem.Domain.Entities.Hotels;
using HotelManagementSystem.Domain.Entities.Rooms;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Data.Configurations
{
    public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.RoomNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(r => r.StatusReason)
                .HasMaxLength(500);

            builder.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(r => r.HotelId)
                .IsRequired();

            builder.Property(r => r.RoomCategoryId)
                .IsRequired();

            builder.HasOne<Hotel>()
                .WithMany()
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<RoomCategory>()
                .WithMany()
                .HasForeignKey(r => r.RoomCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.HotelId);
            
            builder.HasIndex(r => r.RoomCategoryId);

            builder.HasIndex(r => new { r.HotelId, r.RoomNumber })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");


        }
    }
}