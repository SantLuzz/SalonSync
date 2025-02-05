using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Models.Services;

namespace SalonSync.Api.Data.Mappings.Services;

public class ServiceMapping : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
       builder.ToTable("Services");

       builder.HasKey(x => x.Id);
       
       builder.Property(x => x.Name)
           .IsRequired()
           .HasColumnName("Name")
           .HasColumnType("NVARCHAR")
           .HasMaxLength(160);
       
       builder.Property(x => x.Duration)
           .IsRequired()
           .HasColumnName("Duration")
           .HasColumnType("INT")
           .HasDefaultValue(30);

       builder.Property(x => x.Price)
           .IsRequired()
           .HasColumnName("Price")
           .HasColumnType("MONEY")
           .HasDefaultValue(0);
       
       builder.Property(x => x.UserId)
           .IsRequired()
           .HasColumnName("UserId")
           .HasColumnType("NVARCHAR")
           .HasMaxLength(160);
    }
}