using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Models.Financial;

namespace SalonSync.Api.Data.Mappings.Financial;

public class TransactionMapping : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId)
            .IsRequired(true)
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160);
        
        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasColumnName("Description")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired(true)
            .HasColumnName("CreatedAt")
            .HasColumnType("DATETIME2");

        builder.Property(x => x.PaidOrReceivedAt)
            .IsRequired(false)
            .HasColumnName("PaidOrReceivedAt")
            .HasColumnType("DATETIME2");
        
        builder.Property(x => x.Amount)
            .IsRequired(true)
            .HasColumnName("Amount")
            .HasColumnType("MONEY");
        
        builder.Property(x => x.CostCenterId)
            .IsRequired(true)
            .HasColumnName("CostCenterId")
            .HasColumnType("INT");   
    }
}