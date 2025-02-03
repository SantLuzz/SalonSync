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
            .IsRequired()
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR(160)");

        builder.Property(x => x.Description)
            .HasColumnName("Description")
            .HasColumnType("NVARCHAR(255)");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt")
            .HasColumnType("DATETIME2")
            .HasDefaultValueSql("GETDATE()"); // Usa o horário do banco
        
        builder.Property(x => x.PaidOrReceivedAt)
            .HasColumnName("PaidOrReceivedAt")
            .HasColumnType("DATETIME2");
        
        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnName("Amount")
            .HasColumnType("MONEY");
        
        builder.Property(x => x.CostCenterId)
            .IsRequired()
            .HasColumnName("CostCenterId")
            .HasColumnType("BIGINT"); // Melhor usar BIGINT para IDs longos
        
        builder.HasOne(x => x.CostCenter)
            .WithMany()
            .HasForeignKey(x => x.CostCenterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}