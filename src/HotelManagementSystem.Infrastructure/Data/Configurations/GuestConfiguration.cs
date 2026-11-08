using HotelManagementSystem.Domain.Entities.Guests;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using HotelManagementSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Infrastructure.Data.Configurations;

public sealed class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Guest> builder)
    {
        builder.ToTable("Guests");

        builder.HasKey(g => g.Id);

        builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(200);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne<AppUser>()
            .WithOne()
            .HasForeignKey<Guest>(g => g.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(g => g.PhoneNumber)
            .HasConversion(
                phone => phone!.Value,
                value => PhoneNumber.Create(value).Value)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(g => g.Email)
                .HasConversion(
                    email => email!.Value,
                    value => EmailAddress.Create(value).Value)
                .HasMaxLength(254)
                .HasColumnName("Email")
                .IsRequired();

        builder.Property(e => e.DateOfBirth)
            .IsRequired()
            .HasColumnType("date");
    }
}
