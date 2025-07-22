using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Models.Financial;

namespace SalonSync.Api.Data.Mappings.Financial;

public class AccountPayableMapping : IEntityTypeConfiguration<AccountPayable>
{
    public void Configure(EntityTypeBuilder<AccountPayable> builder)
    {
        builder.ToTable("AccountsPayable");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);
        
        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasColumnName("Description")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);
        
        builder.Property(x => x.SupplierName)
            .IsRequired()
            .HasColumnName("SupplierName")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);
        
        builder.Property(x => x.SupplierDocument)
            .IsRequired(false)
            .HasColumnName("SupplierDocument")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(20);
        
        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnName("Amount")
            .HasColumnType("MONEY");
            
        builder.Property(x => x.DueDate)
            .IsRequired()
            .HasColumnName("DueDate")
            .HasColumnType("DATETIME2");
        
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt")
            .HasColumnType("DATETIME2")
            .HasDefaultValueSql("GETDATE()");
        
        builder.Property(x => x.PaymentDate)
            .IsRequired(false)
            .HasColumnName("PaymentDate")
            .HasColumnType("DATETIME2");
    }
}