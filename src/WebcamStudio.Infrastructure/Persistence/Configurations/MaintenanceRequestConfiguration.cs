using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class MaintenanceRequestConfiguration : IEntityTypeConfiguration<MaintenanceRequest>
{
    public void Configure(EntityTypeBuilder<MaintenanceRequest> builder)
    {
        builder.ToTable("MaintenanceRequests");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Description).IsRequired().HasMaxLength(1000);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasOne(r => r.ChecklistRun)
            .WithMany(run => run.MaintenanceRequests)
            .HasForeignKey(r => r.ChecklistRunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.ChecklistItemResult)
            .WithMany()
            .HasForeignKey(r => r.ChecklistItemResultId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Room)
            .WithMany()
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
