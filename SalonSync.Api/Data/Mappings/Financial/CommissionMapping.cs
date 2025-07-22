using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonSync.Core.Models.Financial;

namespace SalonSync.Api.Data.Mappings.Financial;

public class CommissionMapping : IEntityTypeConfiguration<Commission>
{
    public void Configure(EntityTypeBuilder<Commission> builder)
    {
        builder.ToTable("Commissions");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.EmployeeId)
            .IsRequired();
        
        builder.Property(x => x.Date)
            .IsRequired();
        
        builder.Property(x => x.Amount)
            .IsRequired();
    }
}