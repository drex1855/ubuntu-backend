using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebcamStudio.Domain.Entities;

namespace WebcamStudio.Infrastructure.Persistence.Configurations;

public class StoreDebtPaymentConfiguration : IEntityTypeConfiguration<StoreDebtPayment>
{
    public void Configure(EntityTypeBuilder<StoreDebtPayment> builder)
    {
        builder.ToTable("StoreDebtPayments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");

        builder.HasOne(p => p.ModelAccount)
            .WithMany()
            .HasForeignKey(p => p.ModelAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.ModelAccountId);
    }
}
