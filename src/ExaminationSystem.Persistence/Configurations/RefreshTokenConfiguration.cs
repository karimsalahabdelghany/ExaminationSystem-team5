using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
{
    protected override void ConfigureEntity(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.TokenHash)
            .HasMaxLength(512)
            .IsRequired();
        builder.Property(x => x.ExpiryDate)
            .HasColumnType("datetime2")
            .IsRequired();
        builder.Property(x => x.RevokedAt)
            .HasColumnType("datetime2");
        builder.Property(x => x.IpAddress)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.TokenHash);

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
