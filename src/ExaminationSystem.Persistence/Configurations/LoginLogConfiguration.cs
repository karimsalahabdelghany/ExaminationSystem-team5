using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class LoginLogConfiguration : BaseEntityConfiguration<LoginLog>
{
    protected override void ConfigureEntity(EntityTypeBuilder<LoginLog> builder)
    {
        builder.ToTable("LoginLogs");

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.IpAddress)
            .HasMaxLength(64)
            .IsRequired();
        builder.Property(x => x.UserAgent)
            .HasMaxLength(512)
            .IsRequired();
        builder.Property(x => x.Success).IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.LoginLogs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
