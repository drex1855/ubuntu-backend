using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class LoanRequestConfiguration : IEntityTypeConfiguration<LoanRequest>
{
    public void Configure(EntityTypeBuilder<LoanRequest> builder)
    {
        builder.ToTable("LoanRequests");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Amount).HasColumnType("decimal(18,2)");
        builder.Property(r => r.Reason).IsRequired().HasMaxLength(500);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasOne(r => r.RequestedByAccount)
            .WithMany()
            .HasForeignKey(r => r.RequestedByAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.Status, r.RequestedAt });
    }
}
