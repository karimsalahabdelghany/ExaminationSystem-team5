using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class QuizConfiguration : BaseEntityConfiguration<Quiz>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes");

        builder.Property(x => x.DiplomaId).IsRequired();
        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(x => x.Instructions)
            .HasMaxLength(4000)
            .IsRequired();
        builder.Property(x => x.DurationMinutes).IsRequired();
        builder.Property(x => x.PassScore).IsRequired();
        builder.Property(x => x.MaxAttempts).IsRequired();
        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .IsRequired();

        builder.HasIndex(x => x.DiplomaId);

        builder.HasOne(x => x.Diploma)
            .WithMany(x => x.Quizzes)
            .HasForeignKey(x => x.DiplomaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new
            {
                Id = Guid.Parse("A1111111-1111-1111-1111-111111111111"),
                DiplomaId = Guid.Parse("2D21AE7D-D8A0-4F19-9509-F39B5B339A7F"),
                Title = "C# Fundamentals Quiz",
                Instructions = "Answer all questions within the time limit.",
                DurationMinutes = 30,
                PassScore = 60,
                MaxAttempts = 3,
                Status = QuizStatus.Published,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("B2222222-2222-2222-2222-222222222222"),
                DiplomaId = Guid.Parse("8480D832-E7DA-4F56-9A58-91D90A51E683"),
                Title = "Docker Basics Quiz",
                Instructions = "Choose the best answer for each question.",
                DurationMinutes = 20,
                PassScore = 70,
                MaxAttempts = 5,
                Status = QuizStatus.Draft,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            });


    }
}
