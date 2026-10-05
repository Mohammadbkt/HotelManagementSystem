using HotelManagementSystem.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.ReplacedByToken)
            .HasMaxLength(256);

        builder.Property(t => t.DeviceName)
            .HasMaxLength(200);


        builder.Property(t => t.IpAddress)
            .HasMaxLength(45);

        builder.Property(t => t.ExpiresOnUtc).IsRequired();
        builder.Property(t => t.RevokedAt);


        builder.HasIndex(t => t.Token).IsUnique();


        builder.HasIndex(t => t.UserId);


        builder.Ignore(t => t.IsExpired);
        builder.Ignore(t => t.IsActive);

        builder.Ignore(t => t.DomainEvents);
    }
}