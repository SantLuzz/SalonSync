using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Models.Appointments;
using SalonSync.Core.Models.Packages;

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
        
        // builder.HasOne(c => c.ClientPackages)
        //     .WithMany()
        //     .HasForeignKey(c => c.ClientPackageId);
        
        // builder.HasOne(c => c.Appointment)
        //     .WithMany() // ou .WithMany(a => a.ClientPackageAppointments) se o relacionamento for bidirecional
        //     .HasForeignKey(c => c.AppointmentId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}