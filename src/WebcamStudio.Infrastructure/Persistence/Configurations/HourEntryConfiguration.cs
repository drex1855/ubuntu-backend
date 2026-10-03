using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class HourEntryConfiguration : IEntityTypeConfiguration<HourEntry>
{
    public void Configure(EntityTypeBuilder<HourEntry> builder)
    {
        builder.ToTable("HourEntries");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Note).HasMaxLength(500);

        builder.HasOne(h => h.ModelAccount)
            .WithMany()
            .HasForeignKey(h => h.ModelAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(h => h.ModelAccountId);
    }
}
