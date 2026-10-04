using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class StoreSaleConfiguration : IEntityTypeConfiguration<StoreSale>
{
    public void Configure(EntityTypeBuilder<StoreSale> builder)
    {
        builder.ToTable("StoreSales");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");

        builder.HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.ModelAccount)
            .WithMany()
            .HasForeignKey(s => s.ModelAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.ModelAccountId, s.IsCredit });
    }
}
