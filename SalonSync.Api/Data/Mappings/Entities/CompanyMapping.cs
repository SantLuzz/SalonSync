using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Models.Entities;

namespace SalonSync.Api.Data.Mappings.Entities;

public class CompanyMapping : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnName("Name")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);
        
        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);
        
        builder.Property(x => x.Document)
            .IsRequired(false)
            .HasColumnName("Document")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(20);
        
        builder.Property(x => x.Phone)
            .HasColumnName("Phone")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(20);
        
        builder.Property(x => x.Code)
            .IsRequired();
        
        builder.HasIndex(x => x.UserId)
            .IsUnique();
    }
}