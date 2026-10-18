using HotelManagementSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Data.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("Users");

            builder.Property(u => u.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.IsDeleted)
                .HasColumnName("IsDeleted")
                .IsRequired();

            builder.Property(u => u.DeletedAt)
                .HasColumnName("DeletedAt");

            builder.Property(u => u.DeletedBy)
                .HasColumnName("DeletedBy");

            builder.HasIndex(u => u.IsDeleted)
                .HasDatabaseName("IX_Users_IsDeleted");

            builder.HasIndex(u => u.NormalizedEmail)
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0");

        }
    }
}