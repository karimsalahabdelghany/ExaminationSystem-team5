using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class OtpCodeConfiguration : BaseEntityConfiguration<OtpCode>
{
    protected override void ConfigureEntity(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("OtpCodes");

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CodeHash)
            .HasMaxLength(512)
            .IsRequired();
        builder.Property(x => x.Purpose)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .IsRequired();
        builder.Property(x => x.AttemptCount).IsRequired();
        builder.Property(x => x.IsUsed).IsRequired();
        builder.Property(x => x.ExpiresAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.OtpCodes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
