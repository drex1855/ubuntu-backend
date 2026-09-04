using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class ChecklistRunConfiguration : IEntityTypeConfiguration<ChecklistRun>
{
    public void Configure(EntityTypeBuilder<ChecklistRun> builder)
    {
        builder.ToTable("ChecklistRuns");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.AvailableMaterialsNotes).HasMaxLength(1000);
        builder.Property(r => r.MaterialsAttachmentFileName).HasMaxLength(80);
        builder.Property(r => r.MaterialsAttachmentContentType).HasMaxLength(100);

        builder.HasOne(r => r.Room)
            .WithMany(room => room.ChecklistRuns)
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.PerformedByAccount)
            .WithMany(a => a.ChecklistRuns)
            .HasForeignKey(r => r.PerformedByAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.RoomId, r.PerformedAt });
    }
}
