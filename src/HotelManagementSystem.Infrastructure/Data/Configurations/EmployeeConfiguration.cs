using HotelManagementSystem.Domain.Entities.Employee;
using HotelManagementSystem.Domain.Entities.Hotels;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
using HotelManagementSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Data.Configurations
{
    public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(e => e.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<AppUser>()
            .WithOne()
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Role)
            .IsRequired()
            .HasConversion<string>();

            builder.ComplexProperty<PhoneNumber>(h => h.PhoneNumber, p =>
            {
                p.Property(x => x.Value)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("Phone");
            });

            builder.ComplexProperty<EmailAddress>(g => g.EmailAddress, e =>
        {
            e.Property(x => x.Value)
             .IsRequired(false)
             .HasMaxLength(254)
             .HasColumnName("Email");
        });

        }
    }
}