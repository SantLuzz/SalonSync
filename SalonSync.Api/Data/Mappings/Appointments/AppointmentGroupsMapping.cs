using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Models.Appointments;

namespace SalonSync.Api.Data.Mappings.Appointments;

public class AppointmentGroupsMapping : IEntityTypeConfiguration<AppointmentGroup>
{
    public void Configure(EntityTypeBuilder<AppointmentGroup> builder)
    {
        builder.ToTable("AppointmentGroups");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.EmployeeId)
            .IsRequired()
            .HasColumnName("EmployeeId");
        
        builder.Property(x => x.Date)
            .IsRequired()
            .HasColumnName("Date");
        
        builder.Property(x => x.TotalDuration)
            .IsRequired()
            .HasColumnName("TotalDuration");
    }
}