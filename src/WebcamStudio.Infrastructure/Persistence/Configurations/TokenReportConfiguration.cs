using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class TokenReportConfiguration : IEntityTypeConfiguration<TokenReport>
{
    public void Configure(EntityTypeBuilder<TokenReport> builder)
    {
        builder.ToTable("TokenReports");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.TokensAmount).HasColumnType("decimal(18,2)");
        builder.Property(r => r.MonetaryValue).HasColumnType("decimal(18,2)");

        builder.HasOne(r => r.ModelAccount)
            .WithMany(a => a.TokenReports)
            .HasForeignKey(r => r.ModelAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Site)
            .WithMany(s => s.TokenReports)
            .HasForeignKey(r => r.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.ModelAccountId, r.SiteId, r.Period });
    }
}
