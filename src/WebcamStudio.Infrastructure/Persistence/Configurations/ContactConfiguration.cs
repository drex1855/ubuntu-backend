using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName).IsRequired().HasMaxLength(150);
        builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(30);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.Notes).HasMaxLength(2000);
        builder.Property(c => c.Source).IsRequired().HasMaxLength(50);

        builder.HasIndex(c => c.PhoneNumber);
        builder.HasIndex(c => c.Email);

        builder.HasMany(c => c.Tags)
            .WithMany(t => t.Contacts)
            .UsingEntity(j => j.ToTable("ContactTags"));
    }
}
