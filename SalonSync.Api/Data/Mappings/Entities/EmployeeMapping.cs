using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Models.Entities;

namespace SalonSync.Api.Data.Mappings.Entities;

public class EmployeeMapping : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);

        builder.Property(x => x.Phone)
            .IsRequired(false)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(20);
        
        builder.Property(x => x.Position)
            .IsRequired(false)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(50);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt")
            .HasColumnType("DATETIME2")
            .HasDefaultValueSql("GETDATE()");
        
        builder.Property(x => x.UpdatedAt)
            .IsRequired(false)
            .HasColumnName("UpdatedAt")
            .HasColumnType("DATETIME2");

        builder.Property(x => x.Admission)
            .IsRequired(false)
            .HasColumnName("Admission")
            .HasColumnType("DATETIME2");

        builder.Property(x => x.Commission)
            .IsRequired(false)
            .HasColumnName("Commission")
            .HasColumnType("MONEY");

        builder.Property(x => x.Image)
            .IsRequired(false)
            .HasColumnName("Image")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);
        
        builder.Property(x => x.StartTime)
            .IsRequired(false)
            .HasColumnName("StartTime")
            .HasColumnType("TIME");
        
        builder.Property(x => x.EndTime)
            .IsRequired(false)
            .HasColumnName("EndTime")
            .HasColumnType("TIME");
        
        builder.Property(x => x.LunchStart)
            .IsRequired(false)
            .HasColumnName("LunchStart")
            .HasColumnType("TIME");
        
        builder.Property(x => x.LunchEnd)
            .IsRequired(false)
            .HasColumnName("LunchEnd")
            .HasColumnType("TIME");
        
        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);

        builder.Property(x => x.UniqueCode)
            .IsRequired(false);
    }
}