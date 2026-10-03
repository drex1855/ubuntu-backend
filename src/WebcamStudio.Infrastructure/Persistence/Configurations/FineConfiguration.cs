using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class FineConfiguration : IEntityTypeConfiguration<Fine>
{
    public void Configure(EntityTypeBuilder<Fine> builder)
    {
        builder.ToTable("Fines");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Amount).HasColumnType("decimal(18,2)");
        builder.Property(f => f.Reason).IsRequired().HasMaxLength(500);
        builder.Property(f => f.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasOne(f => f.ModelAccount)
            .WithMany()
            .HasForeignKey(f => f.ModelAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => new { f.ModelAccountId, f.Status });
    }
}
