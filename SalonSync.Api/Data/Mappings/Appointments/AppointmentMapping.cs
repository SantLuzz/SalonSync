using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SalonSync.Core.Enums;
using SalonSync.Core.Models.Appointments;

namespace SalonSync.Api.Data.Mappings.Appointments;

public class AppointmentMapping : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);
        
        builder.Property(x => x.Date)
            .IsRequired()
            .HasColumnName("Date")
            .HasColumnType("DATETIME2");

        builder.Property(x => x.StartTime)
            .IsRequired()
            .HasColumnName("StartTime")
            .HasColumnType("TIME");
        
        builder.Property(x => x.EndTime)
            .IsRequired()
            .HasColumnName("EndTime")
            .HasColumnType("TIME");
        
        builder.Property(x => x.Status)
            .IsRequired()
            .HasColumnName("Status")
            .HasColumnType("SMALLINT")
            .HasDefaultValue(EAppointmentStatus.Free);

        builder.Property(x => x.ClientId)
            .IsRequired()
            .HasColumnName("ClientId");
        
        builder.Property(x => x.EmployeeId)
            .IsRequired()
            .HasColumnName("EmployeeId");
        
        builder.Property(x => x.ServiceId)
            .IsRequired()
            .HasColumnName("ServiceId");
    }
}