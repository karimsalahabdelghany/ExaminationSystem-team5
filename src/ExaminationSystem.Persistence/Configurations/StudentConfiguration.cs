using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasData(
            new
            {
                Id = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"),
                UserName = "student@system.com",
                NormalizedUserName = "STUDENT@SYSTEM.COM",
                Email = "student@system.com",
                NormalizedEmail = "STUDENT@SYSTEM.COM",
                EmailConfirmed = true,
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
