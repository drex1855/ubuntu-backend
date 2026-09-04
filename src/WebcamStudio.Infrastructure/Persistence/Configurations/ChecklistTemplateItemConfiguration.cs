using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class ChecklistTemplateItemConfiguration : IEntityTypeConfiguration<ChecklistTemplateItem>
{
    public void Configure(EntityTypeBuilder<ChecklistTemplateItem> builder)
    {
        builder.ToTable("ChecklistTemplateItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name).IsRequired().HasMaxLength(150);
        builder.Property(i => i.Description).HasMaxLength(300);

        builder.HasOne(i => i.Room)
            .WithMany(r => r.TemplateItems)
            .HasForeignKey(i => i.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
