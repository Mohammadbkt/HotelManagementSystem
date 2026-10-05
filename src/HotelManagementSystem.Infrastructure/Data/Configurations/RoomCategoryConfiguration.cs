using HotelManagementSystem.Domain.Entities.Hotels;
using HotelManagementSystem.Domain.Entities.Rooms.RoomCategories;
using HotelManagementSystem.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Data.Configurations;

public sealed class RoomCategoryConfiguration : IEntityTypeConfiguration<RoomCategory>
{
    public void Configure(EntityTypeBuilder<RoomCategory> builder)
    {
        builder.ToTable("RoomCategories");

        builder.HasKey(rc => rc.Id);

        builder.HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(rc => rc.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(rc => rc.Name)
            .HasMaxLength(20);

        builder.Property(rc => rc.Description)
            .HasMaxLength(200);

        builder.Property(rc => rc.MaxCapacity)
            .IsRequired();

        builder.ComplexProperty(rc => rc.BasePricePerNight,
            m => m.ConfigureMoney("Amount", "Currency"));

    }
}
