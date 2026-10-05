using HotelManagementSystem.Domain.Entities.Hotels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelManagementSystem.Infrastructure.Data.Configurations
{
    public sealed class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.ToTable("Hotels");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name)
                    .IsRequired()
                    .HasMaxLength(200);


            builder.ComplexProperty(h => h.Address, a =>
                       {
                           a.Property(x => x.AddressLine1)
                            .IsRequired()
                            .HasMaxLength(200)
                            .HasColumnName("Address_AddressLine1");

                           a.Property(x => x.AddressLine2)
                            .HasMaxLength(200)
                            .HasColumnName("Address_AddressLine2");

                           a.Property(x => x.City)
                            .IsRequired()
                            .HasMaxLength(100)
                            .HasColumnName("Address_City");

                           a.Property(x => x.StateOrProvince)
                            .HasMaxLength(100)
                            .HasColumnName("Address_StateOrProvince");

                           a.Property(x => x.PostalCode)
                            .HasMaxLength(20)
                            .HasColumnName("Address_PostalCode");

                           a.Property(x => x.Country)
                            .IsRequired()
                            .HasMaxLength(100)
                            .HasColumnName("Address_Country");
                       });


            builder.ComplexProperty(h => h.PhoneNumber, p =>
            {
                p.Property(x => x.Value)
                 .IsRequired()
                 .HasMaxLength(20)
                 .HasColumnName("Phone_Number");
            });

        }
    }
}