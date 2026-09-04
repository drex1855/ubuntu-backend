using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class ModelAccountConfiguration : IEntityTypeConfiguration<ModelAccount>
{
    public void Configure(EntityTypeBuilder<ModelAccount> builder)
    {
        builder.ToTable("ModelAccounts");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FullName).IsRequired().HasMaxLength(150);
        builder.Property(a => a.Email).IsRequired().HasMaxLength(200);
        builder.Property(a => a.PhoneNumber).HasMaxLength(30);
        builder.Property(a => a.PasswordHash).IsRequired().HasMaxLength(400);
        builder.Property(a => a.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(a => a.Gender).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(a => a.Email).IsUnique();
        builder.HasIndex(a => a.PhoneNumber).IsUnique();
    }
}
