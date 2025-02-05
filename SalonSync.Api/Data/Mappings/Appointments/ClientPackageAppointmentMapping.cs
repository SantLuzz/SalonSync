using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Models.Appointments;

namespace SalonSync.Api.Data.Mappings.Appointments;

public class ClientPackageAppointmentMapping : IEntityTypeConfiguration<ClientPackageAppointment>
{
    public void Configure(EntityTypeBuilder<ClientPackageAppointment> builder)
    {
        builder.ToTable("ClientsPackagesAppointments");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);
        
        builder.Property(x => x.ClientPackageId)
            .IsRequired()
            .HasColumnName("ClientPackageId");
        
        builder.Property(x => x.AppointmentId)
            .IsRequired()
            .HasColumnName("AppointmentId");
    }
}