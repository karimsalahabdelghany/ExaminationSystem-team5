using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class EnrollmentConfiguration : BaseEntityConfiguration<Enrollment>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.DiplomaId).IsRequired();
        builder.Property(x => x.EnrolledAt)
            .HasColumnType("datetime2")
            .IsRequired();
        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .IsRequired();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.DiplomaId);

        builder.HasOne(x => x.Student)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Diploma)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.DiplomaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
    new
    {
        Id = Guid.Parse("D4E5F6A7-B8C9-0123-DEFA-234567890123"),
        UserId = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"), // Default Student
        DiplomaId = Guid.Parse("2D21AE7D-D8A0-4F19-9509-F39B5B339A7F"), // Backend Engineering Diploma
        EnrolledAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
        Status = EnrollmentStatus.Active,
        CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    },
    new
    {
        Id = Guid.Parse("E5F6A7B8-C9D0-1234-EFAB-345678901234"),
        UserId = Guid.Parse("C3D4E5F6-A7B8-9012-CDEF-123456789012"), // Default Student
        DiplomaId = Guid.Parse("8480D832-E7DA-4F56-9A58-91D90A51E683"), // Cloud & DevOps Diploma
        EnrolledAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
        Status = EnrollmentStatus.Active,
        CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    }
);
    }
}
