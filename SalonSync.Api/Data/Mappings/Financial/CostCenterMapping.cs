using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Enums;
using SalonSync.Core.Models.Financial;

namespace SalonSync.Api.Data.Mappings.Financial;

public class CostCenterMapping : IEntityTypeConfiguration<CostCenter>
{
    public void Configure(EntityTypeBuilder<CostCenter> builder)
    {
        builder.ToTable("CostCenters");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId)
            .IsRequired(true)
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);
        
        builder.Property(x => x.Title)
            .IsRequired(true)
            .HasColumnName("Title")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasColumnName("Description")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);

        builder.Property(x => x.Type)
            .IsRequired(true)
            .HasColumnName("Type")
            .HasColumnType("SMALLINT");
        
        
    }
}