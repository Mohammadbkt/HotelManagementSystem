using HotelManagementSystem.Domain.Entities.Employee;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Domain.ValueObjects.PhoneNumber;
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

            builder.ComplexProperty<PhoneNumber>(h => h.PhoneNumber, p =>
            {
                p.Property(x => x.Value)
                    .IsRequired(false)
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