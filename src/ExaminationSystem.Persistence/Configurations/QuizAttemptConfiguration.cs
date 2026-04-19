using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class QuizAttemptConfiguration : BaseEntityConfiguration<QuizAttempt>
{
    protected override void ConfigureEntity(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("QuizAttempts");

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.QuizId).IsRequired();
        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .IsRequired();
        builder.Property(x => x.StartTime)
            .HasColumnType("datetime2")
            .IsRequired();
        builder.Property(x => x.Deadline)
            .HasColumnType("datetime2")
            .IsRequired();
        builder.Property(x => x.SubmittedAt)
            .HasColumnType("datetime2");

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.QuizId);

        builder.HasOne(x => x.Student)
            .WithMany(x => x.QuizAttempts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quiz)
            .WithMany(x => x.QuizAttempts)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Result)
            .WithOne(x => x.Attempt)
            .HasForeignKey<AttemptResult>(x => x.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
    new
    {
        Id = Guid.Parse("F6A7B8C9-D0E1-2345-FABC-456789012345"),
        UserId = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"), // Default Student
        QuizId = Guid.Parse("A1111111-1111-1111-1111-111111111111"), // C# Fundamentals Quiz
        Status = QuizAttemptStatus.Submitted,
        StartTime = new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc),
        Deadline = new DateTime(2026, 2, 1, 10, 30, 0, DateTimeKind.Utc),
        SubmittedAt = (DateTime?)new DateTime(2026, 2, 1, 10, 25, 0, DateTimeKind.Utc),
        CreatedAt = new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    },
    new
    {
        Id = Guid.Parse("A7B8C9D0-E1F2-3456-ABCD-567890123456"),
        UserId = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"), // Default Student
        QuizId = Guid.Parse("B2222222-2222-2222-2222-222222222222"), // Docker Basics Quiz
        Status = QuizAttemptStatus.InProgress,
        StartTime = new DateTime(2026, 2, 10, 9, 0, 0, DateTimeKind.Utc),
        Deadline = new DateTime(2026, 2, 10, 9, 20, 0, DateTimeKind.Utc),
        SubmittedAt = (DateTime?)null,
        CreatedAt = new DateTime(2026, 2, 10, 9, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    },
    new
    {
        Id = Guid.Parse("B8C9D0E1-F2A3-4567-BCDE-678901234567"),
        UserId = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"), // Default Student
        QuizId = Guid.Parse("A1111111-1111-1111-1111-111111111111"), // C# Fundamentals Quiz
        Status = QuizAttemptStatus.InProgress,
        StartTime = new DateTime(2027, 1, 1, 10, 0, 0, DateTimeKind.Utc),
        Deadline = new DateTime(2027, 12, 31, 23, 59, 59, DateTimeKind.Utc), // Open-ended deadline for testing
        SubmittedAt = (DateTime?)null,
        CreatedAt = new DateTime(2027, 1, 1, 10, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    }
);
    }
}
