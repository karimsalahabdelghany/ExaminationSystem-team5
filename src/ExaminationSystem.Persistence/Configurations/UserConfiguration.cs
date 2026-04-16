using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
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

        builder.HasData(
    new
    {
        Id = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890"),
        UserName = "admin@system.com",
        NormalizedUserName = "ADMIN@SYSTEM.COM",
        Email = "admin@system.com",
        NormalizedEmail = "ADMIN@SYSTEM.COM",
        EmailConfirmed = true,
        // Pre-computed hash for "Admin@123456"
        PasswordHash = "AQAAAAIAAYagAAAAEJ5tQaHbHsOFCiMfQHvFAAFcQUQxkMECnxU2TlxRFiHjlRl3T5UdqKQqTuJdxZw2dA==",
        SecurityStamp = "A1B2C3D4E5F6789012345678901234AB",   // fixed string
        ConcurrencyStamp = "A1B2C3D4-E5F6-7890-ABCD-EF1234567890",
        PhoneNumber = (string?)null,
        PhoneNumberConfirmed = false,
        TwoFactorEnabled = false,
        LockoutEnd = (DateTimeOffset?)null,
        LockoutEnabled = true,
        AccessFailedCount = 0,
        FullName = "System Administrator",
        Status = AccountStatus.Active,
        FailedLoginAttempts = 0,
        LockedUntil = (DateTime?)null,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null,
        RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
    },
    new
    {
        Id = Guid.Parse("B2C3D4E5-F6A7-8901-BCDE-F12345678901"),
        UserName = "instructor@system.com",
        NormalizedUserName = "INSTRUCTOR@SYSTEM.COM",
        Email = "instructor@system.com",
        NormalizedEmail = "INSTRUCTOR@SYSTEM.COM",
        EmailConfirmed = true,
        // Pre-computed hash for "Instructor@123456"
        PasswordHash = "AQAAAAIAAYagAAAAEHmR2VKQJzLkQrN5t3nGWP+4FqvGYj7YGLGv2mN3b8D1oNqNTL2tKfHqR6kPw5wA==",
        SecurityStamp = "B2C3D4E5F6A789012345678901234BCD",
        ConcurrencyStamp = "B2C3D4E5-F6A7-8901-BCDE-F12345678901",
        PhoneNumber = (string?)null,
        PhoneNumberConfirmed = false,
        TwoFactorEnabled = false,
        LockoutEnd = (DateTimeOffset?)null,
        LockoutEnabled = true,
        AccessFailedCount = 0,
        FullName = "Default Instructor",
        Status = AccountStatus.Active,
        FailedLoginAttempts = 0,
        LockedUntil = (DateTime?)null,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null,
        RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
    },
    new
    {
        Id = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"),
        UserName = "student@system.com",
        NormalizedUserName = "STUDENT@SYSTEM.COM",
        Email = "student@system.com",
        NormalizedEmail = "STUDENT@SYSTEM.COM",
        EmailConfirmed = true,
        // Pre-computed hash for "Student@123456"
        PasswordHash = "AQAAAAIAAYagAAAAEK5GJ3Nk8bHfRqM2p7vYWT+9GrwHZk8ZHMHw3pO4c9E2pOqOUM3uLgIrS7lQx6xB==",
        SecurityStamp = "C3D4E5F6A7B890123456789012345CDE",
        ConcurrencyStamp = "C3D4E5F6-A7B8-9012-CDEF-123456789012",
        PhoneNumber = (string?)null,
        PhoneNumberConfirmed = false,
        TwoFactorEnabled = false,
        LockoutEnd = (DateTimeOffset?)null,
        LockoutEnabled = true,
        AccessFailedCount = 0,
        FullName = "Default Student",
        Status = AccountStatus.Active,
        FailedLoginAttempts = 0,
        LockedUntil = (DateTime?)null,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null,
        RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
    }
);

    }
}
