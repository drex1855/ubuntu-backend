using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class ChecklistItemResultConfiguration : IEntityTypeConfiguration<ChecklistItemResult>
{
    public void Configure(EntityTypeBuilder<ChecklistItemResult> builder)
    {
        builder.ToTable("ChecklistItemResults");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(r => r.Observation).HasMaxLength(1000);
        builder.Property(r => r.AttachmentFileName).HasMaxLength(80);
        builder.Property(r => r.AttachmentContentType).HasMaxLength(100);

        builder.HasOne(r => r.ChecklistRun)
            .WithMany(run => run.ItemResults)
            .HasForeignKey(r => r.ChecklistRunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.TemplateItem)
            .WithMany(t => t.Results)
            .HasForeignKey(r => r.TemplateItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
