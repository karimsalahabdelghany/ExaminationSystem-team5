using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.ToTable("Users");

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.EmailConfirmed)
            .IsRequired();

        builder.Property(x => x.UserName)
            .HasMaxLength(256);

        builder.Property(x => x.NormalizedUserName)
            .HasMaxLength(256);

        builder.Property(x => x.NormalizedEmail)
            .HasMaxLength(256);

        builder.Property(x => x.FullName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .IsRequired();

        builder.Property(x => x.FailedLoginAttempts)
            .IsRequired();

        builder.Property(x => x.LockedUntil)
            .HasColumnType("datetime2");

        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(x => x.CreatedBy)
            .HasMaxLength(128)
            .IsRequired();
        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime2");
        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(128);
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();
        builder.Property(x => x.DeletedAt)
            .HasColumnType("datetime2");

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
        
    }
}
