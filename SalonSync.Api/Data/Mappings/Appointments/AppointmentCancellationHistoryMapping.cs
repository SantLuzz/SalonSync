using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Enums;
using SalonSync.Core.Models.Appointments;

namespace SalonSync.Api.Data.Mappings.Appointments;

public class AppointmentCancellationHistoryMapping : IEntityTypeConfiguration<AppointmentCancellationHistory>
{
    public void Configure(EntityTypeBuilder<AppointmentCancellationHistory> builder)
    {
        builder.ToTable("AppointmentsCancellationsHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);

        builder.Property(x => x.AppointmentId)
            .IsRequired();

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
            .HasDefaultValue(EAppointmentStatus.Completed);

        builder.Property(x => x.ClientId)
            .IsRequired(false)
            .HasColumnName("ClientId");
        
        builder.Property(x => x.EmployeeId)
            .IsRequired()
            .HasColumnName("EmployeeId");
        
        builder.Property(x => x.ServiceId)
            .IsRequired(false)
            .HasColumnName("ServiceId");
        
        builder.Property(x => x.AppointmentGroupId)
            .IsRequired(false)
            .HasColumnName("AppointmentGroupId");

        builder.Property(x => x.CancellationReason)
            .IsRequired(false)
            .HasColumnName("CancellationReason")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(-1);

        builder.Property(x => x.CanceledBy)
            .IsRequired(true)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);

        builder.Property(x => x.CanceledAt)
            .IsRequired(true)
            .HasColumnType("DATETIME2");
    }
}