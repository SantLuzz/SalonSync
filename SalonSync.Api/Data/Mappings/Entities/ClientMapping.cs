using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Enums;
using SalonSync.Core.Models.Entities;

namespace SalonSync.Api.Data.Mappings.Entities;

public class ClientMapping : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        
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
        
        builder.Property(x => x.PreferredService)
            .IsRequired(false)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);

        builder.Property(x => x.Observations)
            .IsRequired(false)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);
        
        builder.Property(x => x.Type)
            .IsRequired()
            .HasColumnType("SMALLINT")
            .HasDefaultValue(EClientType.Common);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt")
            .HasColumnType("DATETIME2")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false)
            .HasColumnName("UpdatedAt")
            .HasColumnType("DATETIME2");

        builder.Property(x => x.Birthdate)
            .IsRequired(false)
            .HasColumnName("BirthDate")
            .HasColumnType("DATETIME2");

        builder.Property(x => x.LastProcedure)
            .IsRequired(false)
            .HasColumnName("LastProcedure")
            .HasColumnType("DATETIME2");
        
        builder.Property(x => x.Image)
            .IsRequired(false)
            .HasColumnName("Image")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);
        
        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);
    }
}